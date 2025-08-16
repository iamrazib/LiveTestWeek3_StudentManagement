using System.ComponentModel.DataAnnotations;

namespace StudentManagementMVC.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; } // PK, identity by convention

        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Range(1, 150)]
        public int Age { get; set; }
    }
}
