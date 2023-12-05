using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace XmlIdentity
{
    public class XmlClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public XmlClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity claims = await base.GenerateClaimsAsync(user);
            claims.AddClaim(new Claim("name", "Aris"));
            claims.AddClaim(new Claim("customClaim", "test value"));
            claims.AddClaim(new Claim(ClaimTypes.GivenName, user.Name));
            claims.AddClaim(new Claim(ClaimTypes.Surname, user.Name));
            return claims;
        }
    }
}