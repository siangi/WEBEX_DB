using MongoDB.Bson;
using ArchiveAPI.Attributes;

namespace ArchiveAPI.Models
{
    public class Exhibition
    {
        [SwaggerIgnoreAttribute]
        public ObjectId id { get; set; }

        public int ExhibitId { get; set; }  
        public int[] PhotoIds { get; set; }
    }
}
