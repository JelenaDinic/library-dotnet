using AutoMapper;
using Library.DTOs;
using Library.GenericRepository;
using Library.Models;
using Library.Support;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QRCoder;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Library.Services
{
    public class UserService : IUserService
    {
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly IGenericRepository<User> _repository;
        private readonly UserManager<User> _userManager;
        private readonly UrlEncoder _urlEncoder;
        public IMapper Mapper { get; set; }
        public IConfiguration Configuration { get; set; }

        public UserService(IGenericRepository<User> repository, IConfiguration configuration, UserManager<User> userManager, UrlEncoder urlEncoder)
        {
            _repository = repository;
            Configuration = configuration;
            _userManager = userManager;
            _urlEncoder = urlEncoder;
        }
        public async Task Create(UserRegistrationDTO userRegistrationDTO)
        {
            var user = Mapper.Map<User>(userRegistrationDTO);
            user.UserName = user.Email.ToUpper();
            var result = await _userManager.CreateAsync(user, userRegistrationDTO.Password);
            if (!result.Succeeded) throw new KeyNotFoundException();
        }
        public async Task<bool> CheckIfExist(string email)
        {
            return await _repository.Search(user => user.Email == email).AnyAsync();
        }
        public async Task<User?> CheckLoginCredentials(LoginCredentials loginCredentials)
        {
            foreach (User user in await _repository.GetAll())
            {
                if (user.Email.Equals(loginCredentials.Email) && SecurePasswordHasher.Verify(loginCredentials.Password, user.Password))
                {
                    return user;
                }
            }
            return null;
        }
        public string GenerateJwt(User user)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = Configuration["Jwt:Issuer"],
                Audience = Configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha512Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }
        public async Task Update(User user, UserProfileDTO userProfileDTO)
        {
            user.FirstName = userProfileDTO.FirstName;
            user.LastName = userProfileDTO.LastName;
            user.Password = SecurePasswordHasher.Hash(userProfileDTO.Password);
            _repository.Update(user);
            await _repository.SaveAsync();
        }
        public async Task<User?> GetById(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public async Task<UserDetailsDTO> GetDTOById(int id)
        {
            return Mapper.Map<UserDetailsDTO>(await _repository.GetByIdAsync(id));
        }
        public async Task UploadAvatar(int id, string avatar)
        {
            var user = await GetById(id);
            if (user != null)
            {
                user.Avatar = avatar;
                _repository.Update(user);

                await _repository.SaveAsync();
            }

        }

        public async Task<byte[]> GenerateQRCode(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());            
            var authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(authenticatorKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                authenticatorKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            var code = string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("Library online app"),
                _urlEncoder.Encode(user.Email),
                authenticatorKey
            );

            using var qrGenerator = new QRCodeGenerator();

            var qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            var qrCodeBase64 = new Base64QRCode(qrCode).GetGraphic(
                8,
                SixLabors.ImageSharp.Color.Yellow,
                SixLabors.ImageSharp.Color.DeepPink
            );

            return Convert.FromBase64String(qrCodeBase64); ;
        }
    }
}
