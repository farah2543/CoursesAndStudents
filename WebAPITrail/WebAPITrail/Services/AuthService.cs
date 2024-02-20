using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPITrail.Helpers;
using WebAPITrail.Models;
using System.Security.Claims;
using System.Linq;


namespace WebAPITrail.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager <IdentityRole> _roleManager;
        private readonly JWT _jwt;

        public AuthService(UserManager<ApplicationUser> userManager,IOptions<JWT>jwt, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;

        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {


            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));

           /* foreach (var role in roles)
                roleClaims.Add(new Claim("Roles", role));*/

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if(await _userManager.FindByEmailAsync(model.email)is not null)
            {
                return new AuthModel { Message = "Email is already registered!" };
            }

            if (await _userManager.FindByNameAsync(model.username) is not null)
            {
                return new AuthModel { Message = "username is already registered!" };
            }
            var user = new ApplicationUser {
                UserName = model.username,
                Email = model.email,
                Name = model.name,
                PhoneNumber = model.phone,

            };
            var result = await _userManager.CreateAsync(user,model.password);

            if(!result.Succeeded) {
                var errors = string.Empty;
                foreach(var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors };
            }
            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Role =   "User",
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
            };
            

        }
        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();
            var user = await _userManager.FindByNameAsync(model.Username);

            if(user == null || !await _userManager.CheckPasswordAsync(user,model.Password))          {
                authModel.Message = "username or password is incorrect";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);

            var rolesList = await _userManager.GetRolesAsync(user);
            authModel.IsAuthenticated = true;

            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            authModel.UserName = user.UserName;
            authModel.Email = user.Email;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Role = rolesList[0];


            return authModel;

        }
        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if(user is null || !await _roleManager.RoleExistsAsync(model.Role) )
            {
                return "invalid user ID or Role";
            }

            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                return "user already assigned to Role";

            }
            var result = await _userManager.AddToRoleAsync(user, model.Role);

            return result.Succeeded ? string.Empty : "Something went wrong";


        }

    }


}
