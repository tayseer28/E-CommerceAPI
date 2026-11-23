using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Authentications;
using E_Commerce.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<bool>.Fail(Error.Failure("User Not Found", $"User with {email} not found"));
            else
            {
                return await _userManager.CheckPasswordAsync(user, password);

            }

        }

        public async Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            var user = new ApplicationUser
            {
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                PhoneNumber = registerDto.PhoneNumber,
                DisplayName = registerDto.DisplayName
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var erros = result.Errors.Select(e => new Error(e.Code, e.Description)).ToList();
                return Result<IdentityUserResult>.Fail(erros);
            }
            return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.Email, user.UserName, user.DisplayName));
        }

        public async Task<Result<bool>> EmailExistAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<bool>.Ok(user != null);
        }

        public async Task<Result<IdentityUserResult>> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<IdentityUserResult>.Fail(Error.Failure("User Not Found", $"User with {email} not found"));
            else
                return Result<IdentityUserResult>.Ok(new IdentityUserResult(user.Id, user.DisplayName, user.Email, user.UserName));
        }

        public async Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(u => u.Email == email);
            if (user?.Address is null) return Result<AddressDto>.Fail(Error.NotFound("Address Not Found", $"Address for user with email {email} not found"));

            var address = user.Address;
            return new AddressDto()
            {
                FirstName = address.FirstName,
                LastName = address.LastName,
                City = address.City,
                Country = address.Country,
                Street = address.Street
            };
        }

        public async Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("User Not Found", $"User with {email} not found"); // operator overloading (Result<IReadOnlyList<string>>.Fail(...))

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public async Task<Result<AddressDto>> UpdateOrInsertAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default)
        {
            var user = await _userManager.Users.Include(u => u.Address).FirstOrDefaultAsync(U => U.Email == email);
            if (user?.Address is null)
            {
                //insert
                user.Address = new Address
                {
                    FirstName = addressDto.FirstName,
                    LastName = addressDto.LastName,
                    City = addressDto.City,
                    Country = addressDto.Country,
                    Street = addressDto.Street

                };
            }
            else
            {
                // update 
                user.Address.FirstName = addressDto.FirstName;
                user.Address.LastName = addressDto.LastName;
                user.Address.City = addressDto.City;
                user.Address.Country = addressDto.Country;
                user.Address.Street = addressDto.Street;
            }
            var result =  await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return addressDto;
            else
                return Error.Failure("Failure", string.Join(';', result.Errors.Select(e => e.Description)));
        }
    }

}
