// IClaimService.cs
using ST10393673_PROG6212_POE.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public interface IClaimService
    {
        Task SubmitClaimAsync(ClaimViewModel model);
        Task<IEnumerable<ClaimViewModel>> GetClaimsAsync(string claimIdFilter, string statusFilter, DateTime? dateFilter);
        Task<bool> UpdateClaimStatusAsync(int claimId, string newStatus);
    }
}
