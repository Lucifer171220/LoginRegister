using SocialMediaWeb.Dtos;

namespace SocialMediaWeb.Services.Interfaces
{
    public interface IUserService
    {
        Task RegisterAsync(UserRegistrationDto registrationDto);
        Task<UserResponseDto> LoginAsync(UserLoginDto loginDto);    
    }
}
