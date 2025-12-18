using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HotelVivaBueno.Data.Enums;

namespace HotelVivaBueno.Data.Entities
{
    public class Employee : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string DocumentNumber { get; set; } = string.Empty; // Used for Login

        [JsonIgnore]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string JobTitle { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; }

        public DateTime? DateOfBirth { get; set; } // From Excel

        [MaxLength(50)]
        public string? Shift { get; set; } // From Excel "Turno"

        [MaxLength(50)]
        public string? ContractType { get; set; } // From Excel "TipoContrato"

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        [MaxLength(500)]
        public string? ProfessionalProfile { get; set; }

        public EducationLevel EducationLevel { get; set; } = EducationLevel.Bachelor;

        // Foreign Key
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
