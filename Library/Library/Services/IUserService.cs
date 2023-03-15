using AutoMapper;
using Library.DTOs;
using Library.Models;

namespace Library.Services
{
    public interface IUserService
    {
        IMapper Mapper { get; set; }
        Task Create(UserRegistrationDTO userRegistrationDTO);
        Task<bool> CheckIfExist(string email);
        Task<User?> CheckLoginCredentials(LoginCredentials loginCredentials);
        string GenerateJwt(User user);
        Task Update(User user, UserProfileDTO userProfileDTO);
        Task<User?> GetById(int id);
        Task<UserDetailsDTO> GetDTOById(int id);
        Task UploadAvatar(int id, string avatar);
        Task<byte[]> GenerateQRCode(int userId);
    }
}
