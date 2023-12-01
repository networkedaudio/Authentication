using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlIdentity
{
    internal class CredentialStores
    {
        internal static ConcurrentDictionary<Guid, ApplicationUser> IDs = new ConcurrentDictionary<Guid, ApplicationUser>();
        internal static ConcurrentDictionary<Guid, string> Passwords = new ConcurrentDictionary<Guid, string>();
        internal static ConcurrentDictionary<Guid, PhoneNumber> PhoneNumbers = new ConcurrentDictionary<Guid, PhoneNumber>(); 
        internal static ConcurrentDictionary<Guid, List<RoleTypes>> RoleType = new ConcurrentDictionary<Guid, List<RoleTypes>>();
        internal static ConcurrentDictionary<Guid, IdentityRole> UserRoles = new ConcurrentDictionary<Guid, IdentityRole>();
    }
}
