using Microsoft.AspNetCore.Mvc;
using ST10393673_PROG6212_POE.Models;
using ST10393673_PROG6212_POE.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly BlobStorageService _blobStorageService;

        public ClaimsController(IClaimService claimService, BlobStorageService blobStorageService)
        {
            _claimService = claimService;
            _blobStorageService = blobStorageService;
        }

        // GET: Claims/SubmitClaim
        [HttpGet]
        public IActionResult SubmitClaim()
        {
            return View();
        }

        // POST: Claims/SubmitClaim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClaim(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Upload file to Blob Storage
                if (model.SupportingDocuments != null && model.SupportingDocuments.Length > 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        await model.SupportingDocuments.CopyToAsync(stream);
                        stream.Position = 0; // Reset stream position
                        
                        // Ensure the container name is set appropriately
                        string containerName = "claims-files"; 
                        // Upload to Blob Storage
                        await _blobStorageService.UploadBlobAsync(containerName, model.SupportingDocuments.FileName, stream);
                        
                        // Store the file URL in the claim model
                        model.AttachmentUrl = $"https://{_blobStorageService.AccountName}.blob.core.windows.net/{containerName}/{model.SupportingDocuments.FileName}";
                    }
                }

                // Save the claim
                await _claimService.SubmitClaimAsync(model);
                return RedirectToAction("ViewStatus");
            }

            // If model state is invalid, return the same view with the model to show errors
            return View(model);
        }

        // GET: Claims/ViewStatus
        [HttpGet]
        public async Task<IActionResult> ViewStatus(string claimIdFilter, string statusFilter, DateTime? dateFilter)
        {
            var claims = await _claimService.GetClaimsAsync(claimIdFilter, statusFilter, dateFilter);
            return View(claims);
        }

        // POST: Claims/FilterClaims (for filtering)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterClaims(string claimId, string status, DateTime? submissionDate)
        {
            var claims = await _claimService.GetClaimsAsync(claimId, status, submissionDate);
            return View("ViewStatus", claims); // Return the ViewStatus view with filtered claims
        }

        // POST: Claims/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int claimId, string newStatus)
        {
            var validStatuses = new List<string> { "Pending", "Approved", "Rejected" };
            if (!validStatuses.Contains(newStatus))
            {
                ModelState.AddModelError("", "Invalid status selected.");
                return RedirectToAction("ViewStatus");
            }

            bool updated = await _claimService.UpdateClaimStatusAsync(claimId, newStatus);
            if (!updated)
            {
                ModelState.AddModelError("", "Failed to update claim status.");
            }

            return RedirectToAction("ViewStatus");
        }

        // GET: Claims/VerifyClaims
        [HttpGet]
        public async Task<IActionResult> VerifyClaims()
        {
            var claims = await _claimService.GetClaimsAsync(null, null, null);
            return View(claims);
        }
    }
}
