using AutoMapper;
using Library.DTOs;
using Library.GenericRepository;
using Library.Models;

namespace Library.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IGenericRepository<Author> _repository;
        public IMapper Mapper { get; set; }

        public AuthorService(IGenericRepository<Author> repository)
        {
            _repository = repository;
        }

        public async Task Create(AuthorDTO authorDTO)
        {
            var author = Mapper.Map<Author>(authorDTO);
            await _repository.InsertAsync(author);
        }

        public async Task Update(int authorId, AuthorDTO authorDTO)
        {
            Author? author = await _repository.GetByIdAsync(authorId);

            if (author == null) throw new KeyNotFoundException("Author not found.");

            author = Mapper.Map(authorDTO, author);
            _repository.Update(author);
            await _repository.SaveAsync();
        }

        public async Task<AuthorDetailsDTO?> GetById(int id)
        {
            Author author = await _repository.GetByIdAsync(id);
            AuthorDetailsDTO dto = Mapper.Map<AuthorDetailsDTO>(author);
            return dto;

        }

        public async Task Delete(int id)
        {
            var author = await _repository.GetByIdAsync(id);

            if (author == null) throw new KeyNotFoundException();

            _repository.Delete(author);
            await _repository.SaveAsync();

        }

        public async Task<List<AuthorDetailsDTO>> GetAll()
        {
            List<AuthorDetailsDTO> authorDTOs = new List<AuthorDetailsDTO>();

            foreach(var author in (List<Author>) await _repository.GetAll())
                authorDTOs.Add(Mapper.Map<AuthorDetailsDTO>(author));

            return authorDTOs;
        }
    }
}
