using Microsoft.AspNetCore.DataProtection;

namespace XmlIdentity
{
    public class DPAPIProtection
    {
        internal static IDataProtector CurrentProtector;
        internal static IDataProtector GetProvider()
        {
            if (CurrentProtector == null)
            {
                // get the path to %LOCALAPPDATA%\myapp-keys
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MissionAudioServer");

                // instantiate the data protection system at this folder
                var dataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(path));

                CurrentProtector = dataProtectionProvider.CreateProtector("Program.ForumWebHost");

            }

            return CurrentProtector;
        }
      
        public static string Protect(string protect) 
        {
            return protect;
            return GetProvider().Protect(protect);
        }

        public static string Unprotect(string protect)
        {
            return protect;
            return GetProvider().Unprotect(protect);
        }


    }
}
