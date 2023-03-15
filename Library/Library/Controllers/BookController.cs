using AutoMapper;
using Library.DTOs;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("api/books")]
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookService bookService, ILogger<BookController> logger, IMapper mapper)
            :base(mapper)
        {
            _bookService = bookService;
            _logger = logger;
            _bookService.Mapper = mapper;
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="404">If book does not exist.</response>
        /// <response code="500">If an error occures.</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Finding book with id: {id} .", id);
                var book = await _bookService.GetBookDetailsById(id);

                if (book == null)
                {
                    _logger.LogWarning("Book with id: {id} was not found", id);
                    return NotFound();
                }

                _logger.LogInformation("Book with id: {id} was successfully found", id);
                return Ok(book);
            }
            catch
            {
                _logger.LogError("An error occured while getting book with id : {id}.", id);
                return StatusCode(500);
            }
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="500">If an error occures.</response>
        /// <response code="404">Returns if no book is found..</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Finding all books.");
                var books = await _bookService.GetAll();

                if (books.Count < 1)
                {
                    _logger.LogWarning("No book was found.");
                    return NotFound("No book was found.");
                }
                _logger.LogInformation("All books are found.");
                return Ok(books);
            }
            catch
            {
                _logger.LogWarning("An error occured");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Required role: ADMIN or LIBRARIAN
        /// </summary>
        /// <response code="200">Returns if successful.</response>
        /// <response code="400">If an error occured.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles = "ADMIN, LIBRARIAN")]
        public async Task<ActionResult> Create(BookDTO bookDTO)
        {
            _logger.LogInformation("Trying to add new book.");
            try
            {
                _logger.LogInformation("Saving book {name}.", bookDTO.Name);
                await _bookService.Create(bookDTO);

                _logger.LogInformation("Book {name} is successfully created.", bookDTO.Name);
                return Ok("Book is successfully created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while saving book {name}!", bookDTO.Name);
                return BadRequest();
            }
        }

        /// <summary>
        /// Required role: ADMIN or LIBRARIAN
        /// </summary>
        /// <response code="200">Returns updated book if successful.</response>
        /// <response code="404">If book with passed id does not exist.</response>
        /// <response code="500">If an error occured.</response>
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN, LIBRARIAN")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, BookDTO bookDTO)
        {
            try
            {
                await _bookService.Update(id, bookDTO);

                _logger.LogInformation("Book profile is successfully updated!");
                return Ok(bookDTO);
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest("Book with passed id does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to update book!");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Required role: ADMIN or LIBRARIAN
        /// </summary>
        /// <response code="200">Returns if deleting was successful.</response>
        /// <response code="404">If book with passed id does not exist.</response>
        /// <response code="500">If an error occured.</response>
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles = "ADMIN, LIBRARIAN")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Trying ot delete the book with id: {id}.", id);
                await _bookService.Delete(id);

                _logger.LogInformation("Book is successfully deleted.");
                 return Ok("Book is successfully deleted.");
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest("Book with passed id does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to update book!");
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Required role: ADMIN or LIBRARIAN
        /// </summary>
        /// <response code="200">Returns if successful.</response>
        /// <response code="500">If an error occured.</response>
        [HttpPut("authors")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "ADMIN, LIBRARIAN")]
        public async Task<ActionResult> AddAuthorsToBook(BookAuthorsDTO bookAuthorsDTO)
        {
            try
            {
                bool isFound = await _bookService.AddAuthorsToBook(bookAuthorsDTO);
                if (isFound)
                    return Ok("Author is successfully assigned to book");
                else return BadRequest("There is no book or author with passed id");
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
