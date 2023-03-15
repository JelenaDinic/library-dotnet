using AutoMapper;
using Library.DTOs;
using Library.Models;

namespace Library.Services
{
    public interface IAuthorService
    {
        IMapper Mapper { get; set; }
        Task Create(AuthorDTO authorDTO);
        Task Update(int authorId, AuthorDTO authorDTO);
        Task<AuthorDetailsDTO?> GetById(int id);
        Task<List<AuthorDetailsDTO>> GetAll();
        Task Delete(int id);
    }
}
