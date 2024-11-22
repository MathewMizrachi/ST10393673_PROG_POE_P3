using System.ComponentModel.DataAnnotations;

namespace ST10393673_PROG6212_POE.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Job Title is required.")]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; } // Lecturer, Program Coordinator, HR

        public bool RememberMe { get; set; } = false;
    }
}
