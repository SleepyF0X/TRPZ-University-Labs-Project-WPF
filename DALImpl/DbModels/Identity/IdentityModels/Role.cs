using System;

namespace DAL.DbModels.Identity.IdentityModels
{
    public sealed class Role
    {
        public Guid Id { get; private set; }
        public string RoleName { get; private set; }

        public Role(string roleName)
        {
            Id = Guid.NewGuid();
            RoleName = roleName;
        }
    }
}
