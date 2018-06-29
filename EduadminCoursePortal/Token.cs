using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace EduadminCoursePortal
{
    public interface IToken
    {
        Task<EduAdminAPIClient.Models.Token> GetNewToken();
    }
    public class Token : IToken
    {
        private readonly IMemoryCache _cache;

        public Token(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public async Task<EduAdminAPIClient.Models.Token> GetNewToken()
        {

            EduAdminAPIClient.Models.Token token = null;

            try
            {
                token = _cache.Get<EduAdminAPIClient.Models.Token>("_token");
            }
            catch
            {
                // ignored
            }

            if (token != null)
                return token;

            var client = new EduAdminAPIClient.Client();

            try
            {
                if (Constants.API.userName.Length > 0 && Constants.API.password.Length > 0)
                    token = await client.Authenticate(Constants.API.userName, Constants.API.password);
            }
            catch
            {
                throw new NotImplementedException("The token used is not valid.");
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            _cache.Set("_token", token, cacheEntryOptions);
            return token;
        }
    }
}
