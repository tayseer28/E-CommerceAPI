using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Authentications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IIdentityService
    {
        Task<Result<IdentityUserResult>> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<bool>> CheckPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyList<string>>> GetUserRoles(string email, CancellationToken cancellationToken = default);
        Task<Result<IdentityUserResult>> CreateUserAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);

        Task<Result<bool>> EmailExistAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<AddressDto>> GetUserAddressByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<Result<AddressDto>> UpdateOrInsertAddressAsync(string email, AddressDto addressDto, CancellationToken ct = default);

    }
}
