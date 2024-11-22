using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ST10393673_PROG6212_POE.Models;
using ST10393673_PROG6212_POE.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly BlobStorageService _blobStorageService;
        private readonly ILogger<ClaimsController> _logger;

        private readonly string _containerName = "claims-files"; // You can later load this from config if needed

        public ClaimsController(IClaimService claimService, BlobStorageService blobStorageService, ILogger<ClaimsController> logger)
        {
            _claimService = claimService;
            _blobStorageService = blobStorageService;
            _logger = logger;
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
                // Upload file to Blob Storage if documents exist
                if (model.SupportingDocuments != null && model.SupportingDocuments.Length > 0)
                {
                    try
                    {
                        using (var stream = new MemoryStream())
                        {
                            await model.SupportingDocuments.CopyToAsync(stream);
                            stream.Position = 0; // Reset stream position

                            // Upload to Blob Storage
                            _logger.LogInformation("Uploading file {FileName} to Blob Storage", model.SupportingDocuments.FileName);
                            await _blobStorageService.UploadBlobAsync(_containerName, model.SupportingDocuments.FileName, stream);

                            // Store the file URL in the claim model
                            model.AttachmentUrl = $"https://{_blobStorageService.AccountName}.blob.core.windows.net/{_containerName}/{model.SupportingDocuments.FileName}";

                            _logger.LogInformation("File uploaded successfully, URL: {FileUrl}", model.AttachmentUrl);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Error uploading file to Blob Storage: {ErrorMessage}", ex.Message);
                        ModelState.AddModelError("", "Failed to upload supporting document.");
                        return View(model); // Return view if upload fails
                    }
                }

                // Submit the claim to the service
                await _claimService.SubmitClaimAsync(model);

                _logger.LogInformation("Claim submitted successfully for Claim ID: {ClaimId}", model.ClaimId);
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
