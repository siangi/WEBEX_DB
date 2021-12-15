using Microsoft.AspNetCore.Http;
namespace ArchiveAPI.Models
{
    /// <summary>
    /// represent the File on the Server for a Photo Entry
    /// </summary>
    public class PhotoFile
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }
}
