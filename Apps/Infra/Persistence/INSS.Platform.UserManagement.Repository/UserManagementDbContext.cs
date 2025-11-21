using INSS.Platform.UserManagement.Entities;
using INSS.Platform.UserManagement.Repository.Configurations;
using Microsoft.EntityFrameworkCore;

namespace INSS.Platform.UserManagement.Repository
{
    /// <summary>
    /// Represents the Entity Framework database context for user management.
    /// Provides access to user management entities and configures their mappings.
    /// </summary>
    public class UserManagementDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserManagementDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="Address"/> entities.
        /// </summary>
        public virtual DbSet<Address> Address { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AddressType"/> entities.
        /// </summary>
        public virtual DbSet<AddressType> AddressType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationPolicy"/> entities.
        /// </summary>
        public virtual DbSet<AuthenticationPolicy> AuthenticationPolicy { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationPolicyProvider"/> entities.
        /// </summary>
        public virtual DbSet<AuthenticationPolicyProvider> AuthenticationPolicyProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationProvider"/> entities.
        /// </summary>
        public virtual DbSet<AuthenticationProvider> AuthenticationProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="AuthenticationProviderMetadata"/> entities.
        /// </summary>
        public virtual DbSet<AuthenticationProviderMetadata> AuthenticationProviderMetadata { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Group"/> entities.
        /// </summary>
        public virtual DbSet<Group> Group { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Individual"/> entities.
        /// </summary>
        public virtual DbSet<Individual> Individual { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Organisation"/> entities.
        /// </summary>
        public virtual DbSet<Organisation> Organisation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Party"/> entities.
        /// </summary>
        public virtual DbSet<Party> Party { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartyAuthenticationProviderMetadata"/> entities.
        /// </summary>
        public virtual DbSet<PartyAuthenticationProviderMetadata> PartyAuthenticationProviderMetadata { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartyProductRole"/> entities.
        /// </summary>
        public virtual DbSet<PartyProductRole> PartyProductRole { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartyRelationship"/> entities.
        /// </summary>
        public virtual DbSet<PartyRelationship> PartyRelationship { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="PartyType"/> entities.
        /// </summary>
        public virtual DbSet<PartyType> PartyType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Permission"/> entities.
        /// </summary>
        public virtual DbSet<Permission> Permission { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Product"/> entities.
        /// </summary>
        public virtual DbSet<Product> Product { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ProductAuthenticationPolicyProvider"/> entities.
        /// </summary>
        public virtual DbSet<ProductAuthenticationPolicyProvider> ProductAuthenticationPolicyProvider { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ProductRole"/> entities.
        /// </summary>
        public virtual DbSet<ProductRole> ProductRole { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ProductRoleResourcePermission"/> entities.
        /// </summary>
        public virtual DbSet<ProductRoleResourcePermission> ProductRoleResourcePermission { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RelationshipType"/> entities.
        /// </summary>
        public virtual DbSet<RelationshipType> RelationshipType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Resource"/> entities.
        /// </summary>
        public virtual DbSet<Resource> Resource { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ResourcePermission"/> entities.
        /// </summary>
        public virtual DbSet<ResourcePermission> ResourcePermission { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Role"/> entities.
        /// </summary>
        public virtual DbSet<Role> Role { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleMetadata"/> entities.
        /// </summary>
        public virtual DbSet<RoleMetadata> RoleMetadata { get; set; }

        /// <summary>
        /// Configures the entity mappings for the user management context.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new AddressTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationPolicyConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationPolicyProviderConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationProviderConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationProviderMetadataConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new IndividualConfiguration());
            modelBuilder.ApplyConfiguration(new OrganisationConfiguration());
            modelBuilder.ApplyConfiguration(new PartyConfiguration());
            modelBuilder.ApplyConfiguration(new PartyAuthenticationProviderMetadataConfiguration());
            modelBuilder.ApplyConfiguration(new PartyProductRoleConfiguration());
            modelBuilder.ApplyConfiguration(new PartyRelationshipConfiguration());
            modelBuilder.ApplyConfiguration(new PartyTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PermissionConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAuthenticationPolicyProviderConfiguration());
            modelBuilder.ApplyConfiguration(new ProductRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ProductRoleResourcePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RelationshipTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());
            modelBuilder.ApplyConfiguration(new ResourcePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleMetadataConfiguration());
        }
    }
}
