using Microsoft.AspNetCore.Mvc;
using ArchiveAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace ArchiveAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController: ControllerBase
    {
        private readonly string basePath;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public FileController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
            basePath = _env.ContentRootPath + "/Photos/";
        }

        /// <summary>
        /// gets a list of all files in the Photos Folder. To look at a specific File
        /// the route is /api/Photos/Filename
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetFilenames()
        {
            try
            {
                string[] Filenames = Directory.GetFiles(basePath);
                return new JsonResult(Filenames);  
            } catch
            {
                return new JsonResult("Auf " + basePath + "konnte nicht zugegriffen werden.");
            }
        }

        /// <summary>
        /// Uploads a Photo File to the Server.
        /// If a file with the same name already exists it will throw an exception.
        /// </summary>
        /// <param name="file">The photo</param>
        /// <returns>a Status Message</returns>
        [HttpPost]
        public JsonResult PostFile([FromForm] PhotoFile file)
        {
            try
            {
                var physicalPath = basePath + file.Name;

                using (var stream = new FileStream(physicalPath, FileMode.CreateNew))
                {
                    file.File.CopyTo(stream);
                }

                return new JsonResult("File: " + file.Name + " was uploaded succesfully");
            }
            catch (Exception ex)
            {
                if (ex is IOException)
                {
                    return new JsonResult("Die Datei existiert bereits. Um eine Datei zu aktualisieren, wird die PUT Methode gebraucht");
                } else
                {
                    return new JsonResult(ex.Message);
                }
            }
        }

        /// <summary>
        /// Searches the File with the same name and replaces it.
        /// If it doesn't exist, it will throw an exception. 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPut]
        public JsonResult UpdateFile([FromForm] PhotoFile file)
        {
            if (System.IO.File.Exists(basePath + file.Name))
            {
                // FileMode.Create Overrides anything existing
                using (var stream = new FileStream(basePath + file.Name, FileMode.Create))
                {
                    file.File.CopyTo(stream);
                }

                return new JsonResult("Datei: " + file.Name + " wurde erfolgreich aktualisiert.");  
            } else
            {
                return new JsonResult("File: " + file.Name + " existiert nicht. Um neue Files hochzuladen wird die Post Methode verwendet");
            }

        }
    }
}
