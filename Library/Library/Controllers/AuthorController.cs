using AutoMapper;
using Library.DTOs;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [Route("api/authors")]
    public class AuthorController : BaseController
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorService authorService, ILogger<AuthorController> logger, IMapper mapper)
            : base(mapper)
        {
            _authorService = authorService;
            _logger = logger;
            _authorService.Mapper = mapper;
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="404">If author does not exist.</response>
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
                _logger.LogInformation("Finding author with id: {id} .", id);
                var authorDTO = await _authorService.GetById(id);

                if (authorDTO == null)
                {
                    _logger.LogWarning("Author with id: {id} was not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Author with id: {id} was successfully found.", id);
                return Ok(authorDTO);
            }
            catch
            {
                _logger.LogError("An error occured while getting author with id : {id}.", id);
                return StatusCode(500);
            }
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="500">If an error occures.</response>
        /// <response code="404">Returns if no author is found..</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            try
            {
                _logger.LogInformation("Finding all authors.");
                var authors = await _authorService.GetAll();

                if (authors.Count < 1)
                {
                    _logger.LogWarning("No author was found.");
                    return NotFound();
                }
                _logger.LogInformation("All authors are found.");
                return Ok(authors);
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
        public async Task<ActionResult> Create(AuthorDTO authorDTO)
        {
            _logger.LogInformation("Trying to add new author.");
            try
            {
                _logger.LogInformation("Saving author {name} {lastname}.", authorDTO.FirstName, authorDTO.LastName);
                await _authorService.Create(authorDTO);

                _logger.LogInformation("Author {name} {lastname} is successfully created.", authorDTO.FirstName, authorDTO.LastName);
                return Ok("Author is successfully created.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while saving author {name} {lastname}!", authorDTO.FirstName, authorDTO.LastName);
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
        public async Task<ActionResult> Update(int id, AuthorDTO authorDTO)
        {
            try
            {
                await _authorService.Update(id, authorDTO);

                _logger.LogInformation("Author is successfully updated!");
                return Ok(authorDTO);

            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest("Author with passed id does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to update author!");
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
                _logger.LogInformation("Trying ot delete author with id: {id}.", id);
                await _authorService.Delete(id);

                _logger.LogInformation("Author is successfully deleted.");
                return Ok("Author is successfully deleted.");
            }
            catch (KeyNotFoundException e)
            {
                _logger.LogWarning(e.Message);
                return BadRequest("Author with passed id does not exist.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to update author!");
                return StatusCode(500);
            }
        }
    }
}
