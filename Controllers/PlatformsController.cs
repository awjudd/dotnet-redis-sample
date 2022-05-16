using Microsoft.AspNetCore.Mvc;
using RedisAPI.Data;
using RedisAPI.Models;

namespace RedisAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _platformRepository;

    public PlatformsController(IPlatformRepo platformRepository)
    {
        _platformRepository = platformRepository;
    }

    [HttpGet("{id}", Name="GetPlatformById")]
    public ActionResult<Platform> GetPlatformById(string id)
    {
        var platform = _platformRepository.GetPlatformById(id);

        if (platform == null)
        {
            return NotFound();
        }
        
        return Ok(platform);
    }

    [HttpPost(Name="CreatePlatform")]
    public ActionResult<Platform> CreatePlatform([FromBody]Platform platform)
    {
        _platformRepository.CreatePlatform(platform);
        
        return CreatedAtRoute(nameof(GetPlatformById), new
        {
            Id = platform.Id,
        }, platform);
    }

    [HttpGet(Name = "GetAllPlatforms")]
    public ActionResult<IEnumerable<Platform>> GetAllPlatforms()
    {
        return Ok(_platformRepository.GetAllPlatforms());
    }
}