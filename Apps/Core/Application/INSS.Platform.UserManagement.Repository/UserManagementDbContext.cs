using INSS.Platform.UserManagement.Entities;
using Microsoft.EntityFrameworkCore;

namespace INSS.Platform.UserManagement.Repository
{
    public class UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : DbContext(options)
    {
        public virtual DbSet<IdentityProvider> IdentityProvider { get; set; }

        public virtual DbSet<UserIdentity> UserIdentity { get; set; }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<Organisation> Organisation { get; set; }

        public virtual DbSet<Application> Application { get; set; }

        public virtual DbSet<Role> Role { get; set; }

        public virtual DbSet<OrganisationUser> OrganisationUser { get; set; }

        public virtual DbSet<ApplicationRole> ApplicationRole { get; set; }

        public virtual DbSet<OrganisationUserApplicationRole> OrganisationUserApplicationRole { get; set; }
    }
}
