using System.Linq;
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

        public MongoClient createStandardMongoClient()
        {
            return new MongoClient(_configuration.GetConnectionString("ArchiveConnection"));
        }

        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = createStandardMongoClient();

            var dbList = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").AsQueryable();

            return new JsonResult(dbList);
        }

        [HttpGet("{photoId:int}")]
        public JsonResult Get(int photoId)
        {
            MongoClient dbClient = createStandardMongoClient();

            var filter = Builders<Photo>.Filter.Eq("PhotoId", photoId);

            var findResult = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").Find(filter).ToList();

            return new JsonResult(findResult);
        }

        [HttpPost]
        public JsonResult Post(Photo photo)
        {
            MongoClient dbClient = createStandardMongoClient();

            int LastPhotoId = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").AsQueryable().Count();
            photo.PhotoId = LastPhotoId + 1;

            dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").InsertOne(photo);

            return new JsonResult("Added Succesfully");
        }

        [HttpPut]
        public JsonResult Put(Photo photo)
        {
            MongoClient dbClient = createStandardMongoClient();
            
            var filter = Builders<Photo>.Filter.Eq("PhotoId", photo.Id);
            var update = Builders<Photo>.Update.Set("Description", photo.Description)
                                                .Set("Name", photo.Name)
                                                .Set("Filename", photo.Filename);


            dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").UpdateOne(filter, update);

            return new JsonResult("Updated Succesfully");
        }

    }
}
