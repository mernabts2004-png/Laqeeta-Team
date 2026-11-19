using Microsoft.AspNetCore.Identity;

namespace Laqeeta.API.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Staff" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        throw new Exception(
                            $"Failed to create role: {role}");
                    }
                }
            }
        }

        public static async Task SeedStaffAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var fullName = configuration["SeedStaff:FullName"];
            var email = configuration["SeedStaff:Email"];
            var password = configuration["SeedStaff:Password"];

            await SeedUserAsync(
                userManager,
                fullName,
                email,
                password,
                "Staff");
        }

        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            var fullName = configuration["SeedAdmin:FullName"];
            var email = configuration["SeedAdmin:Email"];
            var password = configuration["SeedAdmin:Password"];

            await SeedUserAsync(
                userManager,
                fullName,
                email,
                password,
                "Admin");
        }

        private static async Task SeedUserAsync(
            UserManager<ApplicationUser> userManager,
            string? fullName,
            string? email,
            string? password,
            string role)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    $"{role} seed configuration is missing.");
            }

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = fullName
                };

                var result = await userManager.CreateAsync(
                    user,
                    password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Failed to create {role} user: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    user,
                    role);

                if (!roleResult.Succeeded)
                {
                    throw new Exception(
                        $"Failed to assign {role} role.");
                }
            }
        }
    }
}