using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace ST10393673_PROG6212_POE.Models
{
    public class ClaimViewModel
    {
        public int ClaimId { get; set; }

        [Required(ErrorMessage = "Lecturer name is required.")]
        public string LecturerName { get; set; }

        [Required(ErrorMessage = "Hours worked is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Enter a valid number of hours.")]
        public int HoursWorked { get; set; }

        [Required(ErrorMessage = "Hourly rate is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Enter a valid hourly rate.")]
        public double HourlyRate { get; set; }

        [Required(ErrorMessage = "Submission date is required.")]
        public DateTime SubmissionDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public IFormFile SupportingDocuments { get; set; }

        // Add AttachmentUrl property
        public string AttachmentUrl { get; set; }
    }
}
