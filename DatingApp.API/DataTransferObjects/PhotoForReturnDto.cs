using System;

namespace DatingApp.API.DataTransferObjects
{
    public class PhotoForReturnDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime UploadedTime { get; set; }
        public bool IsProfilePicture { get; set; }

        public string PublicId { get; set; }
        

    }
}