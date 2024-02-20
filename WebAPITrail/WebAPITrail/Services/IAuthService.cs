using Microsoft.AspNetCore.Mvc;
using WebAPITrail.Models;

namespace WebAPITrail.Services
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);

        Task<string> AddRoleAsync(AddRoleModel model);

    }
}
