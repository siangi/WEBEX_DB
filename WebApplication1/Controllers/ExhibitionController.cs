using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System.Collections;

using MongoDB.Driver;
using ArchiveAPI.Models;


namespace ArchiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExhibitionController: ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public ExhibitionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        private MongoClient createStandardMongoClient()
        {
            return new MongoClient(_configuration.GetConnectionString("ArchiveConnection"));
        }

        /// <summary>
        /// Returns all Exhibtions with the PhotoIDs
        /// </summary>
        [HttpGet]
        public JsonResult getAllExhibitions()
        {
            MongoClient client = createStandardMongoClient();

            var dbList = client.GetDatabase("Exhibitions").GetCollection<Exhibition>("Exhibition").AsQueryable();

            return new JsonResult(dbList);
        }

        /// <summary>
        /// Returns all Photo Objects for a Exhibition
        /// </summary>
        /// <param name="ExhibitId">Given Id of the Exhibition</param>
        /// <returns></returns>
        [HttpGet("{ExhibitId:int}")]
        public JsonResult getExhibitionPhotos(int ExhibitId)
        {
            MongoClient client = createStandardMongoClient();

            var exhibitFilter = Builders<Exhibition>.Filter.Eq("ExhibitId", ExhibitId);

            Exhibition findResult = client.GetDatabase("Exhibitions").GetCollection<Exhibition>("Exhibition").Find(exhibitFilter).First<Exhibition>();

            ArrayList Photos = new ArrayList();

            foreach (int photoID in findResult.PhotoIds)
            {
                var photoFilter = Builders<Photo>.Filter.Eq("PhotoId", photoID);
                var photo = client.GetDatabase("Exhibitions").GetCollection<Photo>("Photos").Find(photoFilter).First();

                Photos.Add(photo);
            }              
            
            return new JsonResult(Photos);
        }


        /// <summary>
        /// Create a new Exhibition in the Database
        /// </summary>
        /// <param name="exhibit"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult postExhibitions(Exhibition exhibit)
        {
            MongoClient dbClient = createStandardMongoClient();
            
            int count = dbClient.GetDatabase("Exhibitions").GetCollection<Exhibition>("Exhibitions").AsQueryable().ToList().Count;
            exhibit.ExhibitId = count;

            dbClient.GetDatabase("Exhibitions").GetCollection<Exhibition>("Exhibition").InsertOne(exhibit);

            return new JsonResult("Added Successfully!");
        }

        /// <summary>
        /// Update a Exhibition in the Database
        /// </summary>
        /// <param name="exhibit"></param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult Put(Exhibition exhibit)
        {
            MongoClient client = createStandardMongoClient();

            var filter = Builders<Exhibition>.Filter.Eq("ExhibitId", exhibit.ExhibitId);
            var update = Builders<Exhibition>.Update.Set("PhotoIds", exhibit.PhotoIds);

            var dbResult = client.GetDatabase("Exhibitions").GetCollection<Exhibition>("Exhibition").UpdateOne(filter, update);

            return new JsonResult(dbResult);
        }
    }
}
