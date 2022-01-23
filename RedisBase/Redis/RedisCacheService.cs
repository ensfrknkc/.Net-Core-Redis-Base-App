using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace RedisBase.Redis
{
    public class RedisCacheService : ICacheManager
    {
        private IDatabase _database;
        private RedisCacheOptions options;
        private string cacheProvider = "Redis";
        private string defaultConnectionString { get; set; }
        private static ConnectionMultiplexer _connectionMultiplexer;

        public RedisCacheService()
        {
            defaultConnectionString = "127.0.0.1:6379";
            options = new RedisCacheOptions
            {
                Configuration = defaultConnectionString
            };
            _connectionMultiplexer = ConnectionMultiplexer.Connect(defaultConnectionString);
        }
        public async Task<bool> Clear()
        {
            try
            {
                var endpoints = _connectionMultiplexer.GetEndPoints(true);
                foreach (var endpoint in endpoints)
                {
                    var server = _connectionMultiplexer.GetServer(endpoint);
                    server.FlushAllDatabases();
                }
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Contains(object key)
        {
            return _database.KeyExists((RedisKey)key);
        }

        public T Get<T>(string cacheKey)
        {
            using (var redisCache = new RedisCache(options))
            {

                var valueString = redisCache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(valueString))
                {
                    var valueObject = JsonConvert.DeserializeObject<T>(valueString);
                    return (T)valueObject;
                }

                return default(T);
            }
        }

        public void Remove(object key)
        {
            using (var redisCache = new RedisCache(options))
            {
                redisCache.Remove((string)key);
            }
        }

        public void Set<T>(string cacheKey, T model)
        {
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(90)
            };

            using (var redisCache = new RedisCache(options))
            {
                var valueString = JsonConvert.SerializeObject(model);
                redisCache.SetString(cacheKey, valueString);
            }
        }
    }
}
