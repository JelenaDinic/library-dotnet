using AutoMapper;
using Library.DTOs;
using Library.Models;

namespace Library.Config
{
    public class MapperProfile : Profile
    {
        public MapperProfile() { 
            CreateMap<UserRegistrationDTO, User>();
            CreateMap<BookDTO, Book>();
            CreateMap<Book, BookDTO>();
            CreateMap<AuthorDTO, Author>();
            CreateMap<AuthorDetailsDTO, Author>();
            CreateMap<Author, AuthorDetailsDTO>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<BookAuthorsDTO, BookAuthor>();
            CreateMap<UserProfileDTO, User>();
            CreateMap<User, UserProfileDTO>();
            CreateMap<UserDetailsDTO, User>();
            CreateMap<User, UserDetailsDTO>();
        }
    }
}
