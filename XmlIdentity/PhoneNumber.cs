using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlIdentity
{
    public class PhoneNumber
    {
        public string Number;
        public bool IsConfirmed;

        public PhoneNumber(string? phoneNumber)
        {
            Number = phoneNumber;
        }

    }
}
