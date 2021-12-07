using MongoDB.Bson;

namespace ArchiveAPI.Models
{
    public class Photo
    {
        public const string PHOTO_URL = "http://localhost:49146/Photos/";
        public ObjectId Id { get; set; }
        public int PhotoId { get; set; }
        public string Filename { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Link { get { return PHOTO_URL + Filename;  } }

    }
}
