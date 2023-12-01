using Microsoft.AspNetCore.Http;
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
        internal static ConcurrentDictionary<Guid, List<RoleTypes>> Roles = new ConcurrentDictionary<Guid, List<RoleTypes>>(); 
    }
}
