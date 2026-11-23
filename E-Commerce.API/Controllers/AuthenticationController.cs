using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.API.Controllers
{

    public class AuthenticationController : ApiBaseController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto, CancellationToken cancellationToken)
            => ToActionResult(await _authenticationService.LoginAsync(loginDto, cancellationToken));

        [HttpPost("register")]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto, CancellationToken ct)
           => ToActionResult(await _authenticationService.RegsiterAsync(registerDto, ct));

        [HttpGet("emailexist")]
        public async Task<ActionResult<bool>> CheckEmail([FromQuery]string email, CancellationToken ct)
            => ToActionResult(await _authenticationService.CheckEmailExistAsync(email, ct));


        [Authorize]
        [HttpGet("currentuser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ??throw new UnauthorizedAccessException("User email claim not found.");
            return ToActionResult(await _authenticationService.GetCurrentUserAsync(email, ct));
        }

        [Authorize]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress(CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException("User email claim not found");
            return ToActionResult(await _authenticationService.GetUserAddressAsync(email, ct));
        }

        [Authorize]
        [HttpPut("address")]
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto addressDto, CancellationToken ct)
        {
            var email = User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException("User email claim not found");

            return ToActionResult(await _authenticationService.UpsertUserAddressAsync(email, addressDto, ct));
        }
          
    }
}
