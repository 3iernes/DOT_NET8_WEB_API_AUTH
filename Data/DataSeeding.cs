using DOT_NET_WEB_API_AUTH.Core.App;
using DOT_NET_WEB_API_AUTH.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DOT_NET_WEB_API_AUTH.Data
{
    public static class DataSeeding
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            //Crear roles
            List<ApplicationRole> roles =
            [
                new ApplicationRole 
                {
                    Id = Guid.NewGuid(), 
                    Name = UserRoles.Admin, 
                    NormalizedName = UserRoles.Admin.ToUpper(), 
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new ApplicationRole 
                { 
                    Id = Guid.NewGuid(), 
                    Name = UserRoles.User, 
                    NormalizedName = UserRoles.User.ToUpper(),
                    ConcurrencyStamp= Guid.NewGuid().ToString()
                }
            ];

            modelBuilder.Entity<ApplicationRole>().HasData(roles);

            //Crear usuarios
            List<ApplicationUser> usuarios =
            [
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@logiports.com.ar",
                    NormalizedEmail = "ADMIN@LOGIPORTS.COM.AR",
                    SecurityStamp = Guid.NewGuid().ToString(),
                },
                new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@logiports.com.ar",
                    NormalizedEmail = "USER@LOGIPORTS.COM.AR",
                    SecurityStamp = Guid.NewGuid().ToString(),
                }
            ];

            modelBuilder.Entity<ApplicationUser>().HasData(usuarios);

            //Agregar PasswordHash a los usuarios
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            usuarios[0].PasswordHash = passwordHasher.HashPassword(usuarios[0], usuarios[0].UserName!);
            usuarios[1].PasswordHash = passwordHasher.HashPassword(usuarios[1], usuarios[1].UserName!);

            //Agregar roles a los usuarios
            List<IdentityUserRole<Guid>> userRoles = new()
            {
                new IdentityUserRole<Guid>
                {
                    UserId = usuarios[0].Id,
                    RoleId = roles.First(r => r.Name == UserRoles.Admin).Id,
                },
                new IdentityUserRole<Guid>
                {
                    UserId = usuarios[0].Id,
                    RoleId = roles.First(r => r.Name == UserRoles.User).Id,
                },
                new IdentityUserRole<Guid>
                {
                    UserId = usuarios[1].Id,
                    RoleId = roles.First(r => r.Name == UserRoles.User).Id,
                }
            };

            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(userRoles);
        }
    }
}
