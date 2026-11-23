using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    internal class AuthenticationService : IAuthenticationService
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public AuthenticationService(IIdentityService identityService,
            ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public Task<Result<bool>> CheckEmailExistAsync(string email, CancellationToken ct = default)
            => _identityService.EmailExistAsync(email, ct);

        public async Task<Result<UserDto>> GetCurrentUserAsync(string email, CancellationToken cancellationToken = default)
        {
            var result = await _identityService.FindByEmailAsync(email, cancellationToken);
            if (!result.IsSuccess)
                return Result<UserDto>.Fail(result.Errors);
            var user = result.data;
            var roles = await _identityService.GetUserRoles(email, cancellationToken);
            var token = _tokenService.CreateToken(user.Id, user.UserName, user.UserName, roles.data);

            return Result<UserDto>.Ok(new UserDto
            {
                Email = result.data.Email,
                DisplayName = result.data.DisplayName,
                Token = token
            });
        }

        public async Task<Result<AddressDto>> GetUserAddressAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _identityService.GetUserAddressByEmailAsync(email, cancellationToken);
            
        }

        public async Task<Result<UserDto>> LoginAsync(LoginDto loginDto, CancellationToken ct = default)
        {
            // get user by email
            var user = await _identityService.FindByEmailAsync(loginDto.Email);
            if (!user.IsSuccess)
                return Result<UserDto>.Fail(user.Errors);
            //check password 
            var passwordCheck = await _identityService.CheckPasswordAsync(loginDto.Email, loginDto.Password);

            if (!passwordCheck.IsSuccess)
                return Result<UserDto>.Fail(passwordCheck.Errors);

            if (!passwordCheck.data) // if password check 
                return Result<UserDto>.Fail(Error.Unauthorized("Invalid Email or Password"));
            //return result + dto
            var roles = await _identityService.GetUserRoles(loginDto.Email);
            var token = _tokenService.CreateToken(user.data.Id, user.data.Email, user.data.UserName, roles.data);
            return new UserDto()
            {
                Email = loginDto.Email,
                DisplayName = user.data.DisplayName,
                Token = token
            };

        }

        public async Task<Result<UserDto>> RegsiterAsync(RegisterDto registerDto, CancellationToken ct = default)
        {
            var result = await _identityService.CreateUserAsync(registerDto, ct);
            if (!result.IsSuccess)
                return Result<UserDto>.Fail(result.Errors);
            var roles = await _identityService.GetUserRoles(registerDto.Email);

            var token = _tokenService.CreateToken(result.data.Id, result.data.Email, result.data.UserName, roles.data);
            return new UserDto()
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                Token = token
            };
        }

        public Task<Result<AddressDto>> UpsertUserAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            return _identityService.UpdateOrInsertAddressAsync(email, addressDto, ct);
        }
    }
}
