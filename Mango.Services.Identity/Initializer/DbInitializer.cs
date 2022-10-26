using System.Security.Claims;
using IdentityModel;
using Mango.Services.Identity.DbContexts;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity.Initializer;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitializer(ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _applicationDbContext = applicationDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task Initialize()
    {
        var adminRole = await _roleManager.FindByNameAsync(Sd.Admin);
        if (adminRole != null) return;

        await _roleManager.CreateAsync(new IdentityRole(Sd.Admin));
        await _roleManager.CreateAsync(new IdentityRole(Sd.Customer));

        #region Admin

        var adminUser = new ApplicationUser
        {
            FirstName = "Juan Miguel",
            LastName = "Paulino Carpio",
            UserName = "juanmiguel431",
            Email = "juanmiguel431@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "18298205436",
        };

        await _userManager.CreateAsync(adminUser, "Admin123*");
        await _userManager.AddToRoleAsync(adminUser, Sd.Admin);

        var adminName = $"{adminUser.FirstName} {adminUser.LastName}";
        await _userManager.AddClaimsAsync(adminUser, new List<Claim>
        {
            new Claim(JwtClaimTypes.Name, adminName),
            new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
            new Claim(JwtClaimTypes.Role, Sd.Admin),
        });

        #endregion

        #region Customer

        var customerUser = new ApplicationUser
        {
            FirstName = "Juan Miguel",
            LastName = "Paulino Carpio",
            UserName = "juanmiguel_431",
            Email = "juanmiguel_431@hotmail.com",
            EmailConfirmed = true,
            PhoneNumber = "18298205436",
        };

        await _userManager.CreateAsync(customerUser, "Customer123*");
        await _userManager.AddToRoleAsync(customerUser, Sd.Customer);

        var customerName = $"{customerUser.FirstName} {customerUser.LastName}";
        await _userManager.AddClaimsAsync(customerUser, new List<Claim>
        {
            new Claim(JwtClaimTypes.Name, customerName),
            new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
            new Claim(JwtClaimTypes.Role, Sd.Customer),
        });

        #endregion
    }
}