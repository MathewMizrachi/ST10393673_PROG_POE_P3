using Microsoft.AspNetCore.Mvc;

namespace ST10393673_PROG6212_POE.Models
{
    public class ClaimStatusHistoryModel
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string Status { get; set; }
        public DateTime ChangeDate { get; set; }
        public string ChangedBy { get; set; } // Admin or user
    }

}
