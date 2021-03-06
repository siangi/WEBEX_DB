using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Collects all Photo Entry from the Database
        /// </summary>
        /// <returns>A List of Photo Entries</returns>
        [HttpGet]
        public JsonResult Get()
        {
            MongoClient dbClient = createStandardMongoClient();

            var dbList = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").AsQueryable();

            return new JsonResult(dbList);
        }

        /// <summary>
        /// calls for a specific Photo Entry 
        /// </summary>
        /// <param name="photoId">PhotoID of the entry</param>
        /// <returns>The Databse entry</returns>
        [HttpGet("{photoId:int}")]
        public JsonResult Get(int photoId)
        {
            MongoClient dbClient = createStandardMongoClient();

            var filter = Builders<Photo>.Filter.Eq("PhotoId", photoId);

            var findResult = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").Find(filter).ToList();

            return new JsonResult(findResult);
        }

        /// <summary>
        /// Add a new Photo to the Database. Only Metadata, not the File itself.
        /// </summary>
        /// <param name="photo">JSON Object with all the Photo Parameters</param>
        /// <returns>Status Message</returns>
        [HttpPost]
        public JsonResult Post(Photo photo)
        {
            MongoClient dbClient = createStandardMongoClient();

            int count = dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").AsQueryable().Count();
            photo.PhotoId = count;


            dbClient.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").InsertOne(photo);

            return new JsonResult("Added Succesfully");
        }

        /// <summary>
        /// Updates a photo Entry in the Database. The PhotoId is used as an identifier, so it shall not be changed.
        /// </summary>
        /// <param name="photo">JSON Object of the Entry</param>
        /// <returns>Status Message</returns>
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

        /// <summary>
        /// Deletes a Photo entry in the Database. 
        /// </summary>
        /// <param name="photoID">photoID of the Entry</param>
        /// <returns>status message</returns>
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
