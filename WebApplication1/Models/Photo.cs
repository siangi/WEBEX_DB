using MongoDB.Bson;

namespace ArchiveAPI.Models
{
    public class Photo
    {
        public const string PHOTO_URL = "https://webex-groupc.azurewebsites.net/Photos/";
        public ObjectId Id { get; set; }
        public int PhotoId { get; set; }
        public int OrderIndex { get; set; }
        public string AltText { get; set; }
        public string TextOffsetX { get; set; }
        public string TextOffsetY { get; set; }
        public int ColumnWidth { get; set; }
        public int RowHeight { get; set; }
        public string Filename { get; set; }
        public string TextTitle { get; set; }
        public string Description { get; set; }



        public string Link { get { return PHOTO_URL + Filename;  } }

    }
}
