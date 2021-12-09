using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.IO;
using ArchiveAPI.Models;


namespace ArchiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public PhotoController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private MongoClient createStandardMongoClient()
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

            int count = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").AsQueryable().Count();
            photo.PhotoId = count;

            dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").InsertOne(photo);

            return new JsonResult("Added Succesfully");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult("File: " + filename + " was uploaded succesfully");
            }
            catch
            {
                return new JsonResult("there was a problem with saving the sent file");
            }
        }

        [HttpPut]
        public JsonResult Put(Photo photo)
        {
            MongoClient dbClient = createStandardMongoClient();
            
            var filter = Builders<Photo>.Filter.Eq("PhotoId", photo.PhotoId);
            var update = Builders<Photo>.Update.Set("OrderIndex", photo.OrderIndex)
                                                .Set("AltText", photo.AltText)
                                                .Set("TextOffsetX", photo.TextOffsetY)
                                                .Set("TextOffsetY", photo.TextOffsetY)
                                                .Set("ColumnWidth", photo.ColumnWidth)
                                                .Set("RowHeight", photo.RowHeight)
                                                .Set("Filename", photo.Filename)
                                                .Set("TextTitle", photo.TextTitle)
                                                .Set("Description", photo.Description);


            dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").UpdateOne(filter, update);

            return new JsonResult("Updated Succesfully");
        }

        [HttpDelete("{PhotoId:int}")]
        public JsonResult Delete(int photoID)
        {
            MongoClient dbClient = createStandardMongoClient();

            var filter = Builders<Photo>.Filter.Eq("PhotoId", photoID);

            var result = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").DeleteOne(filter);

            return new JsonResult(result);

        }

    }
}
