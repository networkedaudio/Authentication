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
        internal static ConcurrentDictionary<Guid, IdentityRole> Roles = new ConcurrentDictionary<Guid, IdentityRole>();
        internal static ConcurrentDictionary<Guid, List<IdentityRole>> UserRoles = new ConcurrentDictionary<Guid, List<IdentityRole>>();
    }
}
