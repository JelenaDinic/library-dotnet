using AutoMapper;
using Library.DTOs;

namespace Library.Services
{
    public interface IBookService
    {
        IMapper Mapper { get; set; }
        Task Create(BookDTO bookDTO);
        Task Update(int bookId, BookDTO bookDTO);
        Task<List<BookDTO>> GetAll();
        Task Delete(int id);
        Task<bool> AddAuthorsToBook(BookAuthorsDTO bookAuthorsDTO);
        Task<BookDetailsDTO?> GetBookDetailsById(int id);
    }
}
