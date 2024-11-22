using System.ComponentModel.DataAnnotations;

namespace ST10393673_PROG6212_POE.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public JobTitle SelectedJobTitle { get; set; }
    }

    public enum JobTitle
    {
        Lecturer,
        ProgramCoordinator,
        HR
    }
}
