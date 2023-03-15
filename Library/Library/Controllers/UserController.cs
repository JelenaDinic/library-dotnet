using AutoMapper;
using Library.DTOs;
using Library.Models;
using Library.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.Controllers
{
    [Route("api/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(IUserService userService, IMapper mapper, ILogger<UserController> logger, UserManager<User> userManager, SignInManager<User> signInManager)
            : base(mapper)
        {
            _userService = userService;
            _userService.Mapper = mapper;
            _logger = logger;
            _userManager= userManager;
            _signInManager= signInManager;
        }

        /// <remarks>
        /// Adds or removes 2FA on user's account.
        /// </remarks>
        [Authorize]
        [HttpPut("auth/2fa")]
        public async Task<ActionResult> AddRemove2FA()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, true);
                return Ok("2FA is successfully enabled.");
            } 
            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.UpdateSecurityStampAsync(user);
            await _userManager.ResetAuthenticatorKeyAsync(user);

            return Ok("2FA is successfully disabled.");  
        }

        /// <remarks>
        /// Scan this QR code with an authentictor app on your mobile phone device.
        /// This will create an account that generates codes for 2fa login.
        /// </remarks>
        [Authorize]
        [HttpGet("auth/2fa/qr-code")]
        public async Task<ActionResult> GenerateQRCode()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("2FA is disabled.");
            }
            var qrCode = await _userService.GenerateQRCode(int.Parse(userId));

            return File(qrCode, "image/png");
        }

        /// <remarks>
        /// Used for login with 2fa code.
        /// </remarks>
        /// <response code="200">Returns token if successful.</response>
        /// <response code="400">If an error occured.</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        [HttpPost("auth/2fa/authentication")]
        public async Task<ActionResult> Login2fa(LoginWithCode loginWithCode)
        {
            var user = await _userManager.FindByEmailAsync(loginWithCode.Email);

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                return BadRequest("Two factor is disabled. Try login without code.");
            }
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginWithCode.Password, false);
            if (!result.Succeeded)
            { 
                return BadRequest("Wrong credentials!"); 
            }

            var success = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                loginWithCode.Code
            );

            if (!success)
            {
                return BadRequest("Wrong 2FA Code!");
            }

            var token = _userService.GenerateJwt(user);
            return Ok(new { token });
        }

        /// <summary>
        /// Required role: ADMIN
        /// </summary>
        /// <response code="200">Returns token if successful.</response>
        /// <response code="409">If email is alredy used.</response>
        /// <response code="400">If an error occured.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult> Create(UserRegistrationDTO userRegistrationDTO)
        {
            _logger.LogInformation("Searching for user with email: {email} ...", userRegistrationDTO.Email);
            if (await _userService.CheckIfExist(userRegistrationDTO.Email))
            {
                _logger.LogWarning("User with email: {email} already exists.", userRegistrationDTO.Email);
                return Conflict("User is already registered!");
            }
            _logger.LogInformation("Saving user with email: {email} ...", userRegistrationDTO.Email);
            await _userService.Create(userRegistrationDTO);

            _logger.LogInformation("User with email: {email} is successfully registered.", userRegistrationDTO.Email);
            return Ok("User is successfully registered.");
        }

        /// <remarks>
        /// Used for login without 2fa code.
        /// </remarks>
        /// <response code="200">If login is successful.</response>
        /// <response code="404">If user with this credentials is not found.</response>
        /// <response code="400">If an error occured.</response>
        [HttpPost("authentication")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginCredentials loginCredentials)
        {
            try
            {
                _logger.LogInformation("Finding user with email: {email}", loginCredentials.Email);
                var user = await _userManager.FindByEmailAsync(loginCredentials.Email);

                if (user != null)
                {
   
                    if (await _userManager.GetTwoFactorEnabledAsync(user)) 
                    {
                        return BadRequest("2FA is enabled. Try login with code.");
                    }
                    var result = await _signInManager.CheckPasswordSignInAsync(user, loginCredentials.Password, false);
                    if (!result.Succeeded) { return BadRequest(); }

                    var token = _userService.GenerateJwt(user);
                    return Ok(new { token});
                }
                else
                {
                    _logger.LogWarning("User with email: {email} and entered password was not found.", loginCredentials.Email);
                    return NotFound("User with entered credentials was not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to login user with email: {email}!", loginCredentials.Email);
                return BadRequest();
            }
        }

        /// <response code="200">Returns updated user if successful.</response>
        /// <response code="400">If an error occured.</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Update(UserProfileDTO userProfileDTO)
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            User oldUser = null;

            if (identity != null)
            {
                _logger.LogInformation("Finding user...");
                oldUser = await _userService.GetById(Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value));
            }
            else return BadRequest("User is not authenticated!");
            try
            {
                _logger.LogInformation("Updating user profile...");
                await _userService.Update(oldUser, userProfileDTO);
                _logger.LogInformation("User profile is successfully updated!");
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while trying to update user profile!");
                return BadRequest();
            }
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="400">If an error occured.</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        [HttpPut("avatar")]
        public async Task<ActionResult> UploadAvatar(IFormFile file)
        {
            _logger.LogInformation("Uploading file: {file} ...", file.FileName);
            if (file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    _logger.LogInformation("Converting to Base64 string...");
                    string avatar = Convert.ToBase64String(fileBytes);
                    try
                    {
                        _logger.LogInformation("Saving uploaded avatar...");
                        var identity = HttpContext.User.Identity as ClaimsIdentity;
                        int userId = Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
                        await _userService.UploadAvatar(userId, avatar);
                        _logger.LogInformation("Avatar is successfully uploaded.");
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occured while saving avatar.");
                        return StatusCode(500);
                    }
                }
            }
            _logger.LogError("Error while uploading avatar.");
            return BadRequest();
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="404">If user does not exist.</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult> GetById()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int userId = Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                _logger.LogInformation("Finding user with id: {id} ...", userId);
                var user = await _userService.GetDTOById(userId);
                _logger.LogInformation("User with id: {id} was successfully found", userId);
                return Ok(user);
            }
            catch
            {
                _logger.LogWarning("User with id: {id} was not found", userId);
                return NotFound();
            }
        }

        /// <response code="200">Returns if successful.</response>
        /// <response code="400">If an error occured.</response>
        /// <response code="404">If user does not exist.</response>
        /// <response code="204">If user does not have avatar uploaded.</response>
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [Authorize]
        [HttpGet("avatar")]
        public async Task<ActionResult> GetAvatar()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            int id = Int32.Parse(identity.FindFirst(ClaimTypes.NameIdentifier).Value);
            try
            {
                
                _logger.LogInformation("Finding user with id: {id} ...", id);

                var user = await _userService.GetById(id);
                if (user != null)
                {
                    _logger.LogInformation("User with id: {id} was successfully found", id);
                    if (user.Avatar != null)
                    {
                        _logger.LogInformation("User with id: {id} has avatar.", id);
                        return File(Convert.FromBase64String(user.Avatar), "image/*");
                    }
                    else
                    {
                        _logger.LogWarning("User with id: {id} does not have avatar.", id);
                        return NoContent();
                    }
                }
                _logger.LogWarning("User with id: {id} was not found.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured!");
                return BadRequest();
            }
        }
    }
}
