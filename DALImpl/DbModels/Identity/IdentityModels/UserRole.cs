using System;

namespace DAL.DbModels.Identity.IdentityModels
{
    public sealed class UserRole
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public User User { get; private set; }
        public Role Role { get; private set; }

        public UserRole(Guid userId, Guid roleId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            RoleId = roleId;
        }
    }
}
