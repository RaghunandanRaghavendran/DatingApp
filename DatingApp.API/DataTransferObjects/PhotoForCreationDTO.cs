using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DataTransferObjects
{
    public class PhotoForCreationDTO
    {
        public string Url { get; set; }
        public string Description { get; set; }
        public string PublicId { get; set; }
        public IFormFile File { get; set; }
        public DateTime UploadedTime { get; set; }

        public PhotoForCreationDTO()
        {
            UploadedTime = DateTime.Now;
        }

    }
}