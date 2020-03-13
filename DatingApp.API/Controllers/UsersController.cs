using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.DataTransferObjects;
using DatingApp.API.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
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
      public async Task<IActionResult> GetUsers() 
      {
          var users = await _repository.GetUsers();
          var usersToReturn = _mapper.Map<IEnumerable<UsersForList>>(users);
          return Ok(usersToReturn);
      }

      [HttpGet("{id}")]
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