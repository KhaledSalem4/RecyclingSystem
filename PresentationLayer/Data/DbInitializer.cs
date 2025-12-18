using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace RecyclingSystem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAdminAccountAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
                Console.WriteLine("✅ Admin role created");
            }

            // Ensure other roles exist
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (!await roleManager.RoleExistsAsync("Collector"))
            {
                await roleManager.CreateAsync(new IdentityRole("Collector"));
            }

            // Get admin credentials from configuration
            var adminEmail = configuration["AdminAccount:Email"];
            var adminPassword = configuration["AdminAccount:Password"];
            var adminFullName = configuration["AdminAccount:FullName"];
            var adminPhone = configuration["AdminAccount:PhoneNumber"];

            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
            {
                throw new InvalidOperationException("Admin credentials not found in configuration.");
            }

            // Check if admin user already exists
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
            
            if (existingAdmin != null)
            {
                // Update existing admin to ensure email is confirmed
                if (!existingAdmin.EmailConfirmed)
                {
                    existingAdmin.EmailConfirmed = true;
                    await userManager.UpdateAsync(existingAdmin);
                    Console.WriteLine($"✅ Admin email confirmed: {adminEmail}");
                }
                
                // Ensure admin has the Admin role
                if (!await userManager.IsInRoleAsync(existingAdmin, "Admin"))
                {
                    await userManager.AddToRoleAsync(existingAdmin, "Admin");
                    Console.WriteLine($"✅ Admin role assigned to: {adminEmail}");
                }
                
                Console.WriteLine($"ℹ️ Admin account already exists: {adminEmail}");
                return;
            }

            // Check if any other admin exists
            var existingAdmins = await userManager.GetUsersInRoleAsync("Admin");
            if (existingAdmins.Any())
            {
                Console.WriteLine($"⚠️ Another admin account already exists. Skipping admin creation.");
                return;
            }

            // Create new admin user
            var adminUser = new ApplicationUser
            {
                FullName = adminFullName ?? "System Administrator",
                Email = adminEmail,
                UserName = adminEmail,
                PhoneNumber = adminPhone,
                EmailConfirmed = true, // Auto-confirm admin email
                PhoneNumberConfirmed = true,
                LockoutEnabled = false, // Admin cannot be locked out
                Points = 0,
                City = "System",
                Street = "Admin",
                BuildingNo = "1",
                Apartment = "1"
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                // Assign Admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
                Console.WriteLine($"✅ Admin account created successfully: {adminEmail}");
                Console.WriteLine($"📧 Email: {adminEmail}");
                Console.WriteLine($"🔑 Password: {adminPassword}");
                Console.WriteLine($"✔️ Email Confirmed: Yes");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"❌ Failed to create admin account: {errors}");
                throw new InvalidOperationException($"Failed to create admin account: {errors}");
            }
        }
    }
}
