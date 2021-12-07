using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ArchiveAPI.Models;

namespace ArchiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public PhotoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = new MongoClient(_configuration.GetConnectionString("ArchiveConnection"));

            var dbList = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").AsQueryable();

            return new JsonResult(dbList);
        }
    }
}
