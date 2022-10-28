using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Mango.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.Identity.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;

    public ProfileService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    }
    
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var subject = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subject);
        var userClaims = await _userClaimsPrincipalFactory.CreateAsync(user);

        var allUserClaims = userClaims.Claims.ToList();

        var claims = allUserClaims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

        claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName));
        claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName));
        if (_userManager.SupportsUserRole)
        {
            var roles = await _userManager.GetRolesAsync(user);

            foreach (var roleName in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, roleName));
                if (_roleManager.SupportsRoleClaims)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role == null) continue;
                    claims.AddRange(await _roleManager.GetClaimsAsync(role));
                }
            }
        }

        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var subject = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(subject);
        context.IsActive = user != null;
    }
}