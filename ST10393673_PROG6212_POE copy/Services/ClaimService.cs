// ClaimService.cs
using ST10393673_PROG6212_POE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ST10393673_PROG6212_POE.Services
{
    public class ClaimService : IClaimService
    {
        private static readonly List<ClaimViewModel> Claims = new List<ClaimViewModel>();

        public async Task SubmitClaimAsync(ClaimViewModel model)
        {
            await Task.Run(() =>
            {
                model.ClaimId = Claims.Count + 1;
                Claims.Add(model);
            });
        }

        public async Task<IEnumerable<ClaimViewModel>> GetClaimsAsync(string claimIdFilter, string statusFilter, DateTime? dateFilter)
        {
            return await Task.Run(() =>
            {
                var filteredClaims = Claims.AsEnumerable();

                if (!string.IsNullOrEmpty(claimIdFilter))
                {
                    if (int.TryParse(claimIdFilter, out int claimId))
                    {
                        filteredClaims = filteredClaims.Where(c => c.ClaimId == claimId);
                    }
                }

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    filteredClaims = filteredClaims.Where(c => c.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
                }

                if (dateFilter.HasValue)
                {
                    filteredClaims = filteredClaims.Where(c => c.SubmissionDate.Date == dateFilter.Value.Date);
                }

                return filteredClaims.ToList();
            });
        }

        public async Task<bool> UpdateClaimStatusAsync(int claimId, string newStatus)
        {
            return await Task.Run(() =>
            {
                var claim = Claims.FirstOrDefault(c => c.ClaimId == claimId);
                if (claim == null)
                {
                    return false;
                }

                claim.Status = newStatus;
                return true;
            });
        }
    }
}
