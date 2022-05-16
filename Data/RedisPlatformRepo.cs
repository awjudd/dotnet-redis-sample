using System.Text.Json;
using RedisAPI.Models;
using StackExchange.Redis;

namespace RedisAPI.Data;

public class RedisPlatformRepo: IPlatformRepo
{
    private readonly IConnectionMultiplexer _redis;

    private const string SetHash = "hashplatform";

    public RedisPlatformRepo(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }
    
    public void CreatePlatform(Platform platform)
    {
        if (platform == null)
        {
            throw new ArgumentNullException(nameof(platform));
        }
       
        var db = _redis.GetDatabase();
        var serializedPlatform = JsonSerializer.Serialize(platform);

        db.HashSet(SetHash, new HashEntry[]
        {
            new HashEntry(platform.Id, serializedPlatform),
        });
    }

    public Platform? GetPlatformById(string id)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        
        var db = _redis.GetDatabase();
        var serializedPlatform = db.HashGet(SetHash, id);

        if (!string.IsNullOrEmpty(serializedPlatform))
        {
            return JsonSerializer.Deserialize<Platform>(serializedPlatform);
        }

        return null;
    }

    public Platform?[] GetAllPlatforms()
    {
        var db = _redis.GetDatabase();
        var completeSet = db.HashGetAll(SetHash);

        if (completeSet.Length > 0)
        {
            return Array.ConvertAll(
                completeSet, 
                val => JsonSerializer.Deserialize<Platform>(val.Value)
            );
        }

        return null;
    }
}