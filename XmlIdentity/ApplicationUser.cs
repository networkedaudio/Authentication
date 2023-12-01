﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlIdentity
{
    public class ApplicationUser : IdentityUser
    {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string Name { get; set; }
            public string Email { get; set; }

        public List<RoleTypes> Roles { get; set; }
    }
}
