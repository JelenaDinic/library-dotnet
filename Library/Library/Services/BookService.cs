using AutoMapper;
using Library.DTOs;
using Library.GenericRepository;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Services
{
    public class BookService : IBookService
    {
        private readonly IGenericRepository<Book> _repository;
        private readonly IGenericRepository<Author> _authorRepository;
        private readonly IGenericRepository<BookAuthor> _bookAuthorRepository;
        public IMapper Mapper { get; set; }

        public  BookService(IGenericRepository<Book> repository, IGenericRepository<BookAuthor> bookAuthorRepository, IGenericRepository<Author> authorRepository) { 
            _repository = repository;
            _bookAuthorRepository = bookAuthorRepository;
            _authorRepository = authorRepository;
        }

        public async Task Create(BookDTO bookDTO)
        {
            var book = Mapper.Map<Book>(bookDTO);
            await _repository.InsertAsync(book);
        }

        public async Task Update(int bookId, BookDTO bookDTO)
        {
            Book? book = await _repository.GetByIdAsync(bookId);

            if (book == null) throw new KeyNotFoundException();

            book = Mapper.Map(bookDTO, book);
            _repository.Update(book);
            await _repository.SaveAsync();
        }

        public async Task<BookDetailsDTO?> GetBookDetailsById(int id)
        {
            var bookAuthors = await _bookAuthorRepository.ExposeTable()
                .Include(x => x.Author)
                .Include(x => x.Book)
                .Where(x => x.BookId == id).ToListAsync();

            List<AuthorDetailsDTO> authors = new List<AuthorDetailsDTO>();

            foreach(var item in bookAuthors)
                authors.Add(Mapper.Map<AuthorDetailsDTO>(item.Author));

            BookDetailsDTO details = new BookDetailsDTO();
            var book = bookAuthors.FirstOrDefault().Book;

            if (book == null) return null;

            details.Authors = authors;
            details.Id= id;
            details.Quantity = book.Quantity;
            details.Language = book.Language;
            details.Name= book.Name;
            return details;
        }

        public async Task Delete(int id)
        {
            var book = await _repository.GetByIdAsync(id);

            if (book == null) throw new KeyNotFoundException();

            _repository.Delete(book);
            await _repository.SaveAsync();
        }

        public async Task<List<BookDTO>> GetAll()
        {
            List<BookDTO> bookDTOs= new List<BookDTO>();
            foreach(var book in (List<Book>)await _repository.GetAll())
            {
                bookDTOs.Add(Mapper.Map<BookDTO>(book));
            }
            return bookDTOs;
        }

        public async Task<bool> AddAuthorsToBook(BookAuthorsDTO bookAuthorsDTO)
        {
            Book? book = await _repository.GetByIdAsync(bookAuthorsDTO.BookId);
            Author? author = await _authorRepository.GetByIdAsync(bookAuthorsDTO.AuthorId);
            if(book == null || author == null) return false;
            BookAuthor bookAuthor = new BookAuthor() { Book = book, Author = author, BookId = bookAuthorsDTO.BookId, AuthorId = bookAuthorsDTO.AuthorId };
            
            await _bookAuthorRepository.InsertAsync(bookAuthor);
            return true;
        }
    }
}
