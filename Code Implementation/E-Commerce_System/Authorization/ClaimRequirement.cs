using Microsoft.AspNetCore.Authorization;

namespace E_Commerce_System.Authorization
{
    public class ClaimRequirement : IAuthorizationRequirement
    {
        public string claimType { get; }
        public string claimValue { get; }

        public ClaimRequirement(string claimType, string claimValue)
        {
            this.claimType = claimType;
            this.claimValue = claimValue;
        }
    }
}
