using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace NAID_Users.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "Permission";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();

                return Task.FromResult(policy);
            }

            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }
    }
}
