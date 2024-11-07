using TransportationApplication.SharedModels;
using TransportationApplication.UserService.DTOs;

namespace TransportationApplication.UserService.Services
{
    public interface IUserServices
    {
        Task<string> LoginUserAsync(LoginViewModel loginViewModel);
        Task<string> GenerateJwtTokenAsync(User user);
    }
}
