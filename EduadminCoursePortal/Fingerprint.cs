using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.Caching.Memory;

namespace EduadminCoursePortal
{
    public class Fingerprint
    {
        private IMemoryCache _cache;
        private IHostingEnvironment _env;

        public Fingerprint(IMemoryCache memoryCache, IHostingEnvironment env)
        {
            _cache = memoryCache;
            _env = env;
        }

        public string Tag(string rootRelativePath)
        {
            var cacheKey = "FingerPrint" + ":" + rootRelativePath;

            var currentCache = _cache.Get(cacheKey);

            if (currentCache != null)
                return currentCache as string;

            string absolute = _env.ContentRootPath + rootRelativePath;

            if (!File.Exists(absolute))
                throw new FileNotFoundException("File not found", absolute);

            DateTime date = File.GetLastWriteTime(absolute);
            var index = absolute.LastIndexOf('/');

            var result = absolute.Insert(index, "/v-" + date.Ticks);

            _cache.Set(cacheKey, result);

            return result;

            throw new NotImplementedException();
        }
    }
}
