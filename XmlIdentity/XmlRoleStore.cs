using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace XmlIdentity
{
    public class XmlRoleStore : IRoleStore<IdentityRole>
    {
        public XmlRoleStore()
        { 
        }

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(role.Id, out var id))
            {
                CredentialStores.Roles.AddOrUpdate(id, role, (key, oldValue) => role);
            }

            CredentialStores.Serialize();

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            if(Guid.TryParse(role.Id, out var id))
            {
                CredentialStores.Roles.TryRemove(id, out var oldRole);
            }

            CredentialStores.Serialize();

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            Guid.TryParse(role.Id, out var id);
            return Task.FromResult(CredentialStores.Roles[id].NormalizedName);
        }

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id);
        }

        public Task<string?> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            if(role.Name != null)
            {
                return Task.FromResult(role.Name);
            }

            Guid.TryParse(role.Id, out var id);
            if(CredentialStores.Roles.TryGetValue(id, out var storedRole))
            {
                return Task.FromResult(storedRole.Name);
            }

            return Task.FromResult<string>(null);
        }

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken)
        {
            Guid.TryParse(role.Id, out var id);
            CredentialStores.Roles.AddOrUpdate(id, role, (key, oldValue) =>
            {
                role.NormalizedName = normalizedName;
                return role;
            });
            CredentialStores.Serialize();

            return Task.CompletedTask;
        }

        public Task SetRoleNameAsync(IdentityRole role, string? roleName, CancellationToken cancellationToken)
        {
            Guid.TryParse(role.Id, out var id);
            CredentialStores.Roles.AddOrUpdate(id, role, (key, oldValue) =>
            {
                role.Name = roleName;
                return role;
            });

            CredentialStores.Serialize();

            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
        {
            Guid.TryParse(role.Name, out var id);
            CredentialStores.Roles.AddOrUpdate(id, role, (key, oldValue) => role);

            CredentialStores.Serialize();

            return Task.FromResult(IdentityResult.Success);
        }

        Task<IdentityRole?> IRoleStore<IdentityRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (Guid.TryParse(roleId, out var roleGuid))
            {
                if (CredentialStores.Roles.TryGetValue(roleGuid, out var identity))
                {
                    return Task.FromResult(identity);
                }
            }

            return Task.FromResult<IdentityRole>(null);
        }

        Task<IdentityRole?> IRoleStore<IdentityRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var foundByName = CredentialStores.Roles.Values.Where(x => x.NormalizedName == normalizedRoleName);
            if (foundByName.Count() > 0) {
                return Task.FromResult(foundByName.First());
            } else
            {
                return Task.FromResult<IdentityRole>(null);
            }
        }
    }
}
