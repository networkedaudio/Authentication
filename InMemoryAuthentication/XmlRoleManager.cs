using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryAuthentication
{
    internal class XmlRoleManager : RoleManager<IdentityUser>
    {
        public XmlRoleManager(IRoleStore<IdentityUser> store, IEnumerable<IRoleValidator<IdentityUser>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<IdentityUser>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
        }
    }
}
