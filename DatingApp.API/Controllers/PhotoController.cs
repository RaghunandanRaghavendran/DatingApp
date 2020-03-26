using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.DataTransferObjects;
using DatingApp.API.Helpers;
using DatingApp.API.Model;
using DatingApp.API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("/users/{userId}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinarySettings;
        private readonly IUserRepository _repo;
        private readonly Cloudinary _cloudinary;
        public PhotoController(IMapper mapper,
                               IOptions<CloudinarySettings> cloudinarySettings,
                               IUserRepository repo)
        {
            _mapper = mapper;
            _cloudinarySettings = cloudinarySettings;
            _repo = repo;

            Account account = new Account(
                _cloudinarySettings.Value.CloudName,
                _cloudinarySettings.Value.ApiKey,
                _cloudinarySettings.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userID,
        [FromForm]PhotoForCreationDTO photoForCreationDTO)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userID);

            var file = photoForCreationDTO.File;
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500)
                                         .Height(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDTO);

            if (!userFromRepo.Photos.Any(u => u.IsProfilePicture))
                photo.IsProfilePicture = true;

            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveChanges())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userID = userID, id = photo.Id }, photoToReturn);
            }
            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userid, int id)
        {
            if (userid != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            //Also check if the photo passed is genuinely user's pic
            var user = await _repo.GetUser(userid);

            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsProfilePicture)
                return BadRequest("The picture is already a main photo");

            var currentMain = await _repo.GetProfilePictureForUser(userid);
            currentMain.IsProfilePicture = false;

            photoFromRepo.IsProfilePicture = true;

            if (await _repo.SaveChanges())
            {
                return NoContent();
            }
            else
            {
                return BadRequest("Could not set the main photo for some reason");
            }
        }

        [HttpDelete("{photoid}")]
        public async Task<IActionResult> DeletePhoto(int userID, int photoid)
        {
            if (userID != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            //Also check if the photo passed is genuinely user's pic
            var user = await _repo.GetUser(userID);

            if (!user.Photos.Any(p => p.Id == photoid))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _repo.GetPhoto(photoid);

            if (photoFromRepo.IsProfilePicture)
                return BadRequest("The profile picture cannot be removed...");

            if (photoFromRepo.PublicId != null)
            {
                // We need to destroy the photo from the cloudinary and the database 
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);

                var result = _cloudinary.Destroy(deletionParams);
                if (result.Result.ToString() == "ok")
                {
                    _repo.Delete(photoFromRepo);
                }
            }

            if(photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveChanges())
                return Ok();
            return BadRequest("Failed to delete the record");
        }

    }
}
