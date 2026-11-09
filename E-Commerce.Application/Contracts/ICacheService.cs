using E_Commerce.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface ICacheService
    {
        Task<string?> GetDataAsync(string cacheKey, CancellationToken ct = default);
        Task setDataAsync(string cacheKey, object cacheValue, TimeSpan? timeToLive = default, CancellationToken ct = default);
    }
}
