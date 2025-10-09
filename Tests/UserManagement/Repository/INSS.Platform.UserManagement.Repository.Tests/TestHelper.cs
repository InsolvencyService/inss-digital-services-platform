using INSS.Platform.UserManagement.Core.Entities;
using Microsoft.Data.SqlClient;
using System.Reflection;

namespace INSS.Platform.UserManagement.Repository.Tests
{
    internal static class TestHelper
    {
        internal static IEnumerable<User> GenerateUsers(int count = 1)
        {
            for (int index = 0; index < count; index++)
            {
                yield return new User
                {
                    Id = Guid.NewGuid(),
                    Email = $"test{index}@example.com",
                    FirstName = $"Test{index}",
                    LastName = $"User{index}",
                    Created = DateTime.UtcNow,
                    CreatedBy = $"UnitTestUser{{index}}",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = $"UnitTestUser{index}",
                    UserIdentityId = Guid.NewGuid()
                };
            }
        }

        internal static IEnumerable<Organisation> GenerateOrganisations(int count = 1)
        {
            for (int index = 0; index < count; index++)
            {
                yield return new Organisation
                {
                    Id = Guid.NewGuid(),
                    Name = $"TestOrganisation{index}",
                    Created = DateTime.UtcNow,
                    CreatedBy = $"UnitTestOrganisationUser{index}",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = $"UnitTestOrganisationUser{index}"
                };
            }
        }

        internal static IEnumerable<Application> GenerateApplications(int count = 1)
        {
            for (int index = 0; index < count; index++)
            {
                yield return new Application
                {
                    Id = Guid.NewGuid(),
                    Name = $"TestApplication{index}",
                    Created = DateTime.UtcNow,
                    CreatedBy = $"UnitTestApplicationUser{index}",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = $"UnitTestApplicationUser{index}"
                };
            }
        }

        internal static IEnumerable<Role> GenerateRoles(int count = 1)
        {
            for (int index = 0; index < count; index++)
            {
                yield return new Role
                {
                    Id = Guid.NewGuid(),
                    Name = $"TestRole{index}",
                    Description = $"TestDescription{index}",
                    Created = DateTime.UtcNow,
                    CreatedBy = $"UnitTestApplicationUser{index}",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = $"UnitTestApplicationUser{index}"
                };
            }
        }

        internal static IEnumerable<OrganisationUser> GenerateOrganisationUsers(Guid organisationId, int userCount = 1)
        {
            for (int index = 0; index < userCount; index++)
            {
                yield return new OrganisationUser
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisationId,
                    UserId = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    CreatedBy = "UnitTestOrganisationUser",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = "UnitTestOrganisationUser"
                };
            }
        }

        internal static OrganisationUser GenerateOrganisationUser(Guid? organisationId = null, Guid? userId = null)
        {
            return new OrganisationUser
            {
                Id = Guid.NewGuid(),
                OrganisationId = organisationId ?? Guid.NewGuid(),
                UserId = userId ?? Guid.NewGuid(),
                Created = DateTime.UtcNow,
                CreatedBy = "UnitTestOrganisationUser",
                Modified = DateTime.UtcNow,
                ModifiedBy = "UnitTestOrganisationUser"
            };
        }

        internal static IEnumerable<ApplicationRole> GenerateApplicationRoles(Guid applicationId, int roleCount = 1)
        {
            for (int index = 0; index < roleCount; index++)
            {
                yield return new ApplicationRole
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = applicationId,
                    RoleId = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    CreatedBy = "UnitTestApplicationRole",
                    Modified = DateTime.UtcNow,
                    ModifiedBy = "UnitTestApplicationRole"
                };
            }
        }

        internal static ApplicationRole GenerateApplicationRole(Guid? applicationId = null, Guid? roleId = null)
        {
            return new ApplicationRole
            {
                Id = Guid.NewGuid(),
                ApplicationId = applicationId ?? Guid.NewGuid(),
                RoleId = roleId ?? Guid.NewGuid(),
                Created = DateTime.UtcNow,
                CreatedBy = "UnitTestApplicationRole",
                Modified = DateTime.UtcNow,
                ModifiedBy = "UnitTestApplicationRole"
            };
        }

        internal static OrganisationUserApplicationRole GenerateOrganisationUserApplicationRole(Guid organisationUserId, Guid applicationRoleId)
        {
            return new OrganisationUserApplicationRole
            {
                Id = Guid.NewGuid(),
                OrganisationUserId = organisationUserId,
                ApplicationRoleId = applicationRoleId,
                Created = DateTime.UtcNow,
                CreatedBy = "UnitTestOrganisationUserApplicationRole",
                Modified = DateTime.UtcNow,
                ModifiedBy = "UnitTestOrganisationUserApplicationRole"
            };
        }

        internal static SqlException CreateSqlException(string message)
        {
            // Get the internal constructor for SqlErrorCollection
            ConstructorInfo? sqlErrorCollectionConstructor = typeof(SqlErrorCollection)
                .GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null)
                ?? throw new InvalidOperationException("Could not find SqlErrorCollection constructor.");

            object sqlErrorCollection = sqlErrorCollectionConstructor.Invoke(null);

            ConstructorInfo sqlExceptionConstructor = typeof(SqlException)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(c =>
                {
                    ParameterInfo[] parameters = c.GetParameters();
                    return parameters.Length > 0 && parameters[0].ParameterType == typeof(string);
                });

            // Create a SqlException with a custom message and the empty SqlErrorCollection
            SqlException sqlException = (SqlException)sqlExceptionConstructor.Invoke([message, sqlErrorCollection, null, Guid.NewGuid()]);
            return sqlException;
        }
    }
}
