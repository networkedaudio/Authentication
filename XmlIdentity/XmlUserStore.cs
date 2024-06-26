﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace XmlIdentity
{
    public class XmlUserStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IQueryableUserStore<ApplicationUser>, IQueryableRoleStore<ApplicationUser>
    {
        public IQueryable<ApplicationUser> Users => CredentialStores.IDs.Values.AsQueryable();

        public IQueryable<ApplicationUser> Roles => (IQueryable<ApplicationUser>)CredentialStores.Roles.Values.AsQueryable();

        public XmlUserStore()
        {
            CredentialStores.Deserialize();
        }


        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = CredentialStores.Roles.Where(x => x.Value.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            var role = roles.First().Value;
            CredentialStores.UserRoles.AddOrUpdate(user.Id, new List<IdentityRole>() { role }, (key, oldValue) =>
            {
                if (!oldValue.Contains(role))
                {
                    oldValue.Add(role);
                }
                return oldValue;
            });

            CredentialStores.Serialize();

            return Task.CompletedTask;
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            CredentialStores.IDs.AddOrUpdate(user.Id, user, (key, oldValue) => user);

            CredentialStores.Serialize();

            return Task.FromResult(IdentityResult.Success);


        }


        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryRemove(user.Id, out var deletedUser))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            CredentialStores.Serialize();

            return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError { Description = "Unable to delete user." } }));
        }


        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            var applicationUsers = CredentialStores.IDs.Where(x => x.Value.NormalizedEmail == normalizedEmail);

            if (applicationUsers.Count() > 0)
            {
                return Task.FromResult(applicationUsers.First().Value);
            }


            return Task.FromResult<ApplicationUser>(null);
        }

        public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            Guid id = new Guid(userId);

            if (CredentialStores.IDs.TryGetValue(id, out ApplicationUser? applicationUser))
            {
                return Task.FromResult(applicationUser);
            }

            // not found

            if (CredentialStores.IDs.TryGetValue(Guid.Empty, out var tempUser))
            {
                return Task.FromResult(tempUser);
            }

            return Task.FromResult<ApplicationUser>(null);

        }



        public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            foreach (var currentIdentity in CredentialStores.IDs.Values)
            {
                if ((currentIdentity.NormalizedUserName?.Equals(normalizedUserName) == true) || (currentIdentity.UserName?.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase) == true))
                {
                    ApplicationUser applicationUser = new ApplicationUser();
                    applicationUser.UserName = normalizedUserName;
                    applicationUser.Id = currentIdentity.Id;
                    return Task.FromResult(applicationUser);
                }
            }

            return Task.FromResult<ApplicationUser>(null);
        }

        public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryGetValue(user.Id, out var applicationUser))
            {
                return Task.FromResult(applicationUser.Email);
            }

            return Task.FromResult<string>(null);
        }

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryGetValue(user.Id, out var applicationUser))
            {
                return Task.FromResult(applicationUser.EmailConfirmed);
            }

            return Task.FromResult(false);
        }

        public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryGetValue(user.Id, out var applicationUser))
            {
                return Task.FromResult(applicationUser.NormalizedEmail);
            }

            return Task.FromResult<string>(null);
        }


        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryGetValue(user.Id, out var applicationUser))
            {
                return Task.FromResult(applicationUser.NormalizedUserName);
            }

            return Task.FromResult<string>(null);
        }



        public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.Passwords.TryGetValue(user.Id, out var password))
            {
                return Task.FromResult(password);
            }

            return Task.FromResult<string>(null);
        }

        public Task<string?> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.PhoneNumbers.TryGetValue(user.Id, out var phoneNumber))
            {
                return Task.FromResult(phoneNumber.Number);
            }

            return Task.FromResult("000-000-000");
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.PhoneNumbers.TryGetValue(user.Id, out var phoneNumber))
            {
                return Task.FromResult(phoneNumber.IsConfirmed);
            }

            return Task.FromResult(false);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
        {

            var roleStrings = new List<string>();

            if (CredentialStores.UserRoles.TryGetValue(user.Id, out var roles))
            {

                foreach (var role in roles)
                {
                    roleStrings.Add(role.Name);
                }
            }
            return Task.FromResult((IList<string>)GetAccumulatedRoles(roleStrings));
        }

        public IList<string> GetAccumulatedRoles(IList<string> existingRoles)
        {
            int highestRole = 0;

            foreach(var role in existingRoles)
            {
                if(Enum.TryParse(typeof(UserRoleEnum), role, out var currentRole))
                {
                    int roleRank = (int)currentRole;

                    if (roleRank > highestRole)
                    {
                        highestRole = roleRank;
                    }
                }
            }

            List<string> returnRoles = new List<string>();

            foreach(var roleType in Enum.GetValues(typeof(UserRoleEnum)))
            {
                if((int)roleType <= highestRole){
                    returnRoles.Add(roleType.ToString());
                }
            }

            return returnRoles;
        }


        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }


        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryGetValue(user.Id, out var foundUser))
            {
                return Task.FromResult(foundUser.Name);
            }

            throw new Exception("User not found");
        }



        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(CredentialStores.Passwords.ContainsKey(user.Id));
        }



        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            var roles = GetRolesAsync(user, cancellationToken).Result;
            return Task.FromResult(roles.Where(x => x.Equals(roleName, StringComparison.OrdinalIgnoreCase)).Count() > 0);
        }


        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
        {
            if (IsInRoleAsync(user, roleName, cancellationToken).Result)
            {
                List<IdentityRole> updatedList = new List<IdentityRole>();

                if (CredentialStores.UserRoles.TryGetValue(user.Id, out var roleList))
                {
                    foreach (var identityRole in roleList)
                    {
                        if (!identityRole.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase))
                        {
                            updatedList.Add(identityRole);
                        }
                    }
                }

                CredentialStores.UserRoles.AddOrUpdate(user.Id, updatedList, (key, oldValue) => updatedList);

                CredentialStores.Serialize();



            }
            return Task.CompletedTask;
        }

        public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
        {
            // var foundUser = FindByIdAsync(user.Id.ToString(), cancellationToken).Result;
            user.Email = email;

            CredentialStores.IDs.AddOrUpdate(user.Id, user, (key, oldValue) =>
            {
                oldValue.Email = email;
                return oldValue;
            });

            CredentialStores.Serialize();

            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;

            CredentialStores.IDs.AddOrUpdate(user.Id, user, (key, oldValue) =>
            {
                oldValue.EmailConfirmed = confirmed;
                return oldValue;
            });

            CredentialStores.Serialize();
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedEmail;

            CredentialStores.IDs.AddOrUpdate(user.Id, user, (key, oldValue) =>
            {
                oldValue.NormalizedEmail = normalizedEmail;
                return oldValue;
            });


            CredentialStores.Serialize();
            return Task.CompletedTask;
        }



        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;

            CredentialStores.IDs.AddOrUpdate(user.Id, user, (key, oldValue) =>
            {
                oldValue.NormalizedUserName = normalizedName;
                return oldValue;
            });

            CredentialStores.Serialize();
            return Task.CompletedTask;
        }


        public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            CredentialStores.Passwords.AddOrUpdate(user.Id, passwordHash, (key, oldValue) => passwordHash);
            CredentialStores.Serialize();
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberAsync(ApplicationUser user, string? phoneNumber, CancellationToken cancellationToken)
        {
            CredentialStores.PhoneNumbers.AddOrUpdate(user.Id, new PhoneNumber(phoneNumber), (key, oldValue) =>
            {
                oldValue.Number = phoneNumber;
                return oldValue;
            });
            CredentialStores.Serialize();
            return Task.CompletedTask;
        }

        public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
        {
            CredentialStores.PhoneNumbers[user.Id].IsConfirmed = confirmed;
            CredentialStores.Serialize();
            return Task.CompletedTask;
        }



        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            user.Name = userName;

            CredentialStores.IDs.AddOrUpdate(user.Id, user, (key, oldValue) =>
            {
                oldValue.Name = userName;
                return oldValue;
            });
            CredentialStores.Serialize();
            return Task.CompletedTask;
        }


        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            if (CredentialStores.IDs.TryGetValue(user.Id, out var existingValue))
            {
                if (CredentialStores.IDs.TryUpdate(user.Id, user, existingValue))
                {
                    return Task.FromResult(IdentityResult.Success);
                }
                else
                {
                    return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError { Description = "Unable to update user." } }));
                }
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError[] { new IdentityError { Description = "Unable to find user before update." } }));
            }

        }

        Task<IList<ApplicationUser>> IUserRoleStore<ApplicationUser>.GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            IList<ApplicationUser> users = new List<ApplicationUser>();


            foreach (var user in CredentialStores.IDs)
            {
                if (roleName.Equals("any", StringComparison.OrdinalIgnoreCase))
                {
                    users.Add(user.Value);
                }
                else
                {
                    if (CredentialStores.UserRoles.ContainsKey(user.Key))
                    {
                        if (CredentialStores.UserRoles[user.Key].Where(x => x.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase)).Any())
                        {
                            users.Add(user.Value);
                        }
                    }
                }
            }
            return Task.FromResult<IList<ApplicationUser>>(users);

        }

        public Task<string> GetRoleIdAsync(ApplicationUser role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetRoleNameAsync(ApplicationUser role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetRoleNameAsync(ApplicationUser role, string? roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedRoleNameAsync(ApplicationUser role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(ApplicationUser role, string? normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
