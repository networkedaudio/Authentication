using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace XmlIdentity
{
    internal class CredentialStores
    {
        internal static string CredentialFile = "c:\\JSON\\Credentials.json";

        internal static ConcurrentDictionary<Guid, ApplicationUser> IDs = new ConcurrentDictionary<Guid, ApplicationUser>();
        internal static ConcurrentDictionary<Guid, string> Passwords = new ConcurrentDictionary<Guid, string>();
        internal static ConcurrentDictionary<Guid, PhoneNumber> PhoneNumbers = new ConcurrentDictionary<Guid, PhoneNumber>(); 
        internal static ConcurrentDictionary<Guid, IdentityRole> Roles = new ConcurrentDictionary<Guid, IdentityRole>();
        internal static ConcurrentDictionary<Guid, List<IdentityRole>> UserRoles = new ConcurrentDictionary<Guid, List<IdentityRole>>();

        internal static void Deserialize()
        {
            if(File.Exists(CredentialFile))
            {
                foreach(var line in File.ReadAllLines(CredentialFile))
                {
                    int firstBracket = line.IndexOf('{');
                    string identifier = line.Substring(0,firstBracket) ;

                    switch (identifier.Trim())
                    {
                        case "IDs:":
                            IDs = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, ApplicationUser>>(line.Substring(firstBracket));
                            break;
                        case "Passwords:":
                            Passwords = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, string>>(line.Substring(firstBracket));
                            break;
                        case "Phone Numbers:":
                            PhoneNumbers = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, PhoneNumber>>(line.Substring(firstBracket));
                            break;
                        case "Roles:":
                            Roles = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, IdentityRole>>(line.Substring(firstBracket));
                            break;
                        case "User Roles:":
                            UserRoles = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, List<IdentityRole>>>(line.Substring(firstBracket));
                            break;
                    }
                }
            }
        }
        internal static void Serialize()
        {

            StringBuilder jsonDocument = new StringBuilder();

            jsonDocument.AppendLine("IDs: " + JsonSerializer.Serialize(IDs));
            jsonDocument.AppendLine("Passwords: " +JsonSerializer.Serialize(Passwords));
            jsonDocument.AppendLine("Phone Numbers: " + JsonSerializer.Serialize(PhoneNumbers));
            jsonDocument.AppendLine("Roles: " + JsonSerializer.Serialize(Roles));
            jsonDocument.AppendLine("User Roles: " + JsonSerializer.Serialize(UserRoles));

            File.WriteAllText(CredentialFile, jsonDocument.ToString());
        }
    }
}
