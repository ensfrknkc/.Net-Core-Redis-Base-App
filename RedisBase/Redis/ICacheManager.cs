using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisBase.Redis
{
    public interface ICacheManager
    {
        void Set<T>(string cacheKey, T model);

        Task<bool> Clear();

        T Get<T>(string cacheKey);

        bool Contains(object key);

        void Remove(object key);
    }
}
