using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

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
            if (IDs.Count == 0)
            {
                if (File.Exists(CredentialFile))
                {
                    foreach (var line in File.ReadAllLines(CredentialFile))
                    {
                        int firstBracket = line.IndexOf(':') + 1;

                        string identifier = line.Substring(0, firstBracket - 1);

                        switch (identifier.Trim())
                        {
                            case "IDs":
                                IDs = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, ApplicationUser>>(DPAPIProtection.Unprotect(line.Substring(firstBracket).Trim()));
                                break;
                            case "Passwords":
                                Passwords = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, string>>(DPAPIProtection.Unprotect(line.Substring(firstBracket).Trim()));
                                break;
                            case "Phone Numbers":
                                PhoneNumbers = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, PhoneNumber>>(DPAPIProtection.Unprotect(line.Substring(firstBracket).Trim()));
                                break;
                            case "Roles":
                                Roles = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, IdentityRole>>(DPAPIProtection.Unprotect(line.Substring(firstBracket).Trim()));
                                break;
                            case "User Roles":
                                UserRoles = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, List<IdentityRole>>>(DPAPIProtection.Unprotect(line.Substring(firstBracket).Trim()));
                                break;
                        }
                    }
                }
            }
        }
        internal static void Serialize()
        {

            StringBuilder jsonDocument = new StringBuilder();

            jsonDocument.AppendLine("IDs: " + DPAPIProtection.Protect(JsonSerializer.Serialize(IDs)));
            jsonDocument.AppendLine("Passwords: " + DPAPIProtection.Protect(JsonSerializer.Serialize(Passwords)));
            jsonDocument.AppendLine("Phone Numbers: " + DPAPIProtection.Protect(JsonSerializer.Serialize(PhoneNumbers)));
            jsonDocument.AppendLine("Roles: " + DPAPIProtection.Protect(JsonSerializer.Serialize(Roles)));
            jsonDocument.AppendLine("User Roles: " + DPAPIProtection.Protect(JsonSerializer.Serialize(UserRoles)));

            File.WriteAllText(CredentialFile, jsonDocument.ToString());
        }
    }
}
