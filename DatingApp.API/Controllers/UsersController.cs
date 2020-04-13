using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.DataTransferObjects;
using DatingApp.API.Helpers;
using DatingApp.API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
      public UsersController(IUserRepository repository,IMapper mapper)
      {
          _repository = repository;
          _mapper = mapper;
      }

      [HttpGet]
      public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams) 
      {
          // For filtering.We are setting two new Id's in UserParams which are used for filtering
          // setting UserID and Gender so that we can filter own profile appearing and Opposite gender
          int currentuserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
          var userFromRepo = await _repository.GetUser(currentuserID);
          userParams.UserID = currentuserID;
          if(string.IsNullOrEmpty(userParams.Gender))
          {
              userParams.Gender = userFromRepo.Gender == "male"?"female":"male";
          }

          
          var users = await _repository.GetUsers(userParams);
          var usersToReturn = _mapper.Map<IEnumerable<UsersForList>>(users);
          Response.AddPagination(users.CurrentPage, users.PageSize,users.TotalCount,users.TotalPage);
          return Ok(usersToReturn);
      }

      [HttpGet("{id}", Name="GetUser")] // Name used for CreatedAtRoute in AuthControl.
      public async Task<IActionResult> GetUser(int id)
      {
          var user = await _repository.GetUser(id);
          var userToReturn = _mapper.Map<UserForDetails>(user);
          return Ok(userToReturn);
      }

      [HttpPut("{id}")]
      public async Task<IActionResult> UpdateUser(int id, UserForUpdate userForUpdate)
      {
          if(id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
          return Unauthorized();

          var userFromRepo = await _repository.GetUser(id);

          _mapper.Map(userForUpdate, userFromRepo);

          if(await _repository.SaveChanges())
          return NoContent();

          throw new System.Exception($"Updating user {id} failed on save");
      }
    }
}