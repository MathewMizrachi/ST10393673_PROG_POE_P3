using Microsoft.AspNetCore.Mvc;

namespace ST10393673_PROG6212_POE.Models
{
    public class BlobFileMetadataModel
    {
        public string FileName { get; set; }
        public string Url { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }
    }

}
