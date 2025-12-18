-- Insertar usuario admin de prueba para Railway PostgreSQL
-- Username (DocumentNumber): 1234567890
-- Password: Admin123!

INSERT INTO "Employees" (
    "FirstName", 
    "LastName", 
    "Email", 
    "PhoneNumber", 
    "DocumentNumber", 
    "PasswordHash", 
    "JobTitle", 
    "Salary", 
    "HireDate", 
    "Status", 
    "ProfessionalProfile", 
    "EducationLevel", 
    "DepartmentId", 
    "CreatedAt",
    "DateOfBirth",
    "Shift",
    "ContractType"
) VALUES (
    'Admin',
    'Sistema',
    'admin@hotelvidabuena.com',
    '3001234567',
    '1234567890',
    '$2a$11$YourHashedPasswordHere',  -- Se debe generar con BCrypt real
    'Administrador',
    5000000,
    CURRENT_TIMESTAMP,
    0,  -- EmployeeStatus.Active
    'Usuario administrador del sistema',
    3,  -- EducationLevel.Bachelor
    1,  -- Recursos Humanos
    CURRENT_TIMESTAMP,
    NULL,
    NULL,
    NULL
);
