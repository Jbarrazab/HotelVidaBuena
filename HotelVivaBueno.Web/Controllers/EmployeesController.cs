using HotelVivaBueno.Data.Data;
using HotelVivaBueno.Data.Entities;
using HotelVivaBueno.Data.Enums;
using HotelVivaBueno.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HotelVivaBueno.Web.Controllers
{
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string searchString)
        {
            var employees = from e in _context.Employees.Include(e => e.Department)
                            select e;

            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(s => s.LastName.Contains(searchString)
                                       || s.FirstName.Contains(searchString)
                                       || s.DocumentNumber.Contains(searchString));
            }

            return View(await employees.ToListAsync());
        }

        // GET: Employees/Import
        public IActionResult Import()
        {
            return View();
        }

        // POST: Employees/Import
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(ImportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Por favor seleccione un archivo.");
                return View(model);
            }

            // Validate file extension
            var extension = Path.GetExtension(model.File.FileName).ToLowerInvariant();
            if (extension != ".xlsx")
            {
                ModelState.AddModelError("File", "Solo se permiten archivos .xlsx");
                return View(model);
            }

            int successCount = 0;
            int errorCount = 0;
            var errors = new List<string>();

            try
            {
                using (var stream = model.File.OpenReadStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        if (package.Workbook.Worksheets.Count == 0)
                        {
                            ModelState.AddModelError("File", "El archivo Excel no contiene hojas de trabajo.");
                            return View(model);
                        }

                        var worksheet = package.Workbook.Worksheets[0];
                        
                        if (worksheet.Dimension == null)
                        {
                            ModelState.AddModelError("File", "La hoja de trabajo está vacía.");
                            return View(model);
                        }

                        var rowCount = worksheet.Dimension.Rows;
                        var colCount = worksheet.Dimension.Columns;

                        // Validate minimum columns (13 required)
                        if (colCount < 13)
                        {
                            ModelState.AddModelError("File", $"El archivo debe tener al menos 13 columnas. Se encontraron {colCount}.");
                            return View(model);
                        }

                        // Cache departments to avoid multiple DB queries
                        var departmentCache = new Dictionary<string, Department>();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Map columns based on specification
                                // 1:Documento, 2:Nombres, 3:Apellidos, 4:FechaNacimiento, 5:Telefono
                                // 6:Email, 7:Cargo, 8:Departamento, 9:Turno, 10:FechaIngreso
                                // 11:TipoContrato, 12:Salario, 13:Estado

                                var docNumber = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                                if (string.IsNullOrEmpty(docNumber))
                                {
                                    errors.Add($"Fila {row}: Documento vacío, se omite.");
                                    continue;
                                }
                                
                                var firstName = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? "";
                                var lastName = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? "";
                                
                                // Dates
                                DateTime dateOfBirth = DateTime.UtcNow;
                                var dobVal = worksheet.Cells[row, 4].Value;
                                if (dobVal is double d1) dateOfBirth = DateTime.FromOADate(d1);
                                else if (dobVal is DateTime dt1) dateOfBirth = dt1;
                                
                                var phone = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? "";
                                var email = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? "";
                                var jobTitle = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? "";
                                var deptName = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? "General";
                                var shift = worksheet.Cells[row, 9].Value?.ToString()?.Trim();
                                
                                DateTime hireDate = DateTime.UtcNow;
                                var hireVal = worksheet.Cells[row, 10].Value;
                                if (hireVal is double d2) hireDate = DateTime.FromOADate(d2);
                                else if (hireVal is DateTime dt2) hireDate = dt2;

                                var contractType = worksheet.Cells[row, 11].Value?.ToString()?.Trim();
                                
                                decimal salary = 0;
                                var salVal = worksheet.Cells[row, 12].Value;
                                if (salVal != null) decimal.TryParse(salVal.ToString(), out salary);

                                var statusStr = worksheet.Cells[row, 13].Value?.ToString()?.Trim() ?? "Activo";

                                // Find or Create Department (using cache)
                                Department dept;
                                if (!departmentCache.TryGetValue(deptName, out dept))
                                {
                                    dept = await _context.Departments.FirstOrDefaultAsync(d => d.Name == deptName);
                                    if (dept == null)
                                    {
                                        dept = new Department { Name = deptName, Description = "Imported from Excel" };
                                        _context.Departments.Add(dept);
                                        await _context.SaveChangesAsync(); // Save immediately to get ID
                                    }
                                    departmentCache[deptName] = dept;
                                }

                                // Upsert Employee
                                var employee = await _context.Employees
                                    .FirstOrDefaultAsync(e => e.DocumentNumber == docNumber);

                                bool isNewEmployee = false;
                                if (employee == null)
                                {
                                    isNewEmployee = true;
                                    employee = new Employee
                                    {
                                        DocumentNumber = docNumber,
                                        HireDate = hireDate.ToUniversalTime(),
                                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(docNumber) // Default pass = Document
                                    };
                                    _context.Employees.Add(employee);
                                }

                                // Parse Status Enum
                                EmployeeStatus statusEnum = EmployeeStatus.Active;
                                if (statusStr.Contains("Vacaciones", StringComparison.OrdinalIgnoreCase)) 
                                    statusEnum = EmployeeStatus.OnVacation;
                                else if (statusStr.Contains("Incapacidad", StringComparison.OrdinalIgnoreCase) || 
                                         statusStr.Contains("Inactivo", StringComparison.OrdinalIgnoreCase)) 
                                    statusEnum = EmployeeStatus.Inactive;

                                // Update fields
                                employee.FirstName = firstName;
                                employee.LastName = lastName;
                                employee.DateOfBirth = dateOfBirth.ToUniversalTime();
                                employee.PhoneNumber = phone;
                                employee.Email = email;
                                employee.JobTitle = jobTitle;
                                employee.Department = dept;
                                employee.Shift = shift;
                                employee.ContractType = contractType;
                                employee.Salary = salary;
                                employee.Status = statusEnum;

                                successCount++;
                            }
                            catch (Exception ex)
                            {
                                errorCount++;
                                errors.Add($"Fila {row}: {ex.Message}");
                            }
                        }
                        
                        // Save all changes at once
                        await _context.SaveChangesAsync();
                    }
                }

                // Provide feedback to user
                if (successCount > 0)
                {
                    TempData["SuccessMessage"] = $"✅ Importación completada: {successCount} empleado(s) procesado(s) exitosamente.";
                }
                
                if (errorCount > 0)
                {
                    var errorSummary = string.Join("<br/>", errors.Take(10)); // Show max 10 errors
                    if (errors.Count > 10)
                    {
                        errorSummary += $"<br/>... y {errors.Count - 10} error(es) más.";
                    }
                    TempData["ErrorMessage"] = $"⚠️ {errorCount} fila(s) con errores:<br/>{errorSummary}";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al procesar el archivo: {ex.Message}");
                return View(model);
            }
        }

        public IActionResult Create()
        {
            ViewData["Departments"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Set default password hash if needed, or handle validation
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.DocumentNumber); 
                employee.HireDate = employee.HireDate.ToUniversalTime();
                if(employee.DateOfBirth.HasValue) employee.DateOfBirth = employee.DateOfBirth.Value.ToUniversalTime();

                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Departments"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            ViewData["Departments"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Update only modified fields or attached entity
                    // For simplicity in this Clean Simple test, we update full entity but be careful with Pwd
                    var existing = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
                    if (existing != null)
                    {
                        employee.PasswordHash = existing.PasswordHash; // Keep password
                        employee.HireDate = employee.HireDate.ToUniversalTime();
                        if(employee.DateOfBirth.HasValue) employee.DateOfBirth = employee.DateOfBirth.Value.ToUniversalTime();
                        
                        _context.Update(employee);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Employees.Any(e => e.Id == employee.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Departments"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Departments, "Id", "Name", employee.DepartmentId);
            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
}
}
