using AutoMapper;
using BCrypt.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMediaWeb.Dtos;
using SocialMediaWeb.Models;
using SocialMediaWeb.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialMediaWeb.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(ApplicationDbContext context, IMapper mapper, IConfiguration config, IWebHostEnvironment webHostEnvironment) { 
            _context = context;
            _mapper = mapper;
            _config = config;   
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<UserResponseDto> LoginAsync(UserLoginDto loginDto)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }


            var userResponseDto = _mapper.Map<UserResponseDto>(user);
                var jwt = GenerateJwt(user);
                userResponseDto.Token = jwt;

                return userResponseDto;
        }
       

        public async Task RegisterAsync(UserRegistrationDto model)
        {
            
            string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
            string fileExtension = Path.GetExtension(model.ImageFile.FileName);
            string uniqueFileName = $"{fileName}_{DateTime.Now.Ticks}{fileExtension}";
            string filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", uniqueFileName);
            await System.IO.File.WriteAllTextAsync(filePath, model.Image);

            var user = _mapper.Map<User>(model);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password, BCrypt.Net.BCrypt.GenerateSalt());
            user.ImagePath = filePath; // Save image path to database

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
               
            }
            catch (DbUpdateException e)
            {
                _ = e.Message;
            }
        }
    

        private string GenerateJwt(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            _ = int.TryParse(_config["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);


            var token = new JwtSecurityToken(
            issuer: _config["JWT:ValidIssuer"],
            audience: _config["JWT:ValidAudience"],
            expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
            claims: claims,
            signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

    }
}
 