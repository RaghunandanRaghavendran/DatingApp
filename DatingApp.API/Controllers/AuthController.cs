using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DataTransferObjects;
using DatingApp.API.Model;
using DatingApp.API.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository authRepo, IConfiguration config, IMapper mapper)
        {
            _authRepo = authRepo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister registorDTO)
        {
            registorDTO.UserName = registorDTO.UserName.ToLower();

            if(await _authRepo.UserExists(registorDTO.UserName))
            return BadRequest("Username already taken");

            User userToCreate = _mapper.Map<User>(registorDTO);

            User registereduser = await _authRepo.Register(userToCreate,registorDTO.Password);

            var userToReturn = _mapper.Map<UserForDetails>(registereduser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = registereduser.Id}, userToReturn);
            //return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLogin loginDto)
        {
            // We need to compare the user in the database and compare the password which is Hashed
            // Once the validation is done, we need to return the Jwt back to the client                      
            loginDto.UserName = loginDto.UserName.ToLower();
            User loginuser =  await _authRepo.Login(loginDto.UserName,loginDto.Password);
            
            if(loginuser == null)
            {
                return Unauthorized();
            }
            
            //Jwt Token generation

            var claims = new[]
            {
              new Claim(ClaimTypes.NameIdentifier,loginuser.Id.ToString()),
              new Claim(ClaimTypes.Name,loginuser.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                                              (_config.GetSection("AppSettings:Token").Value));

            var signCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = signCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();  

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UsersForList>(loginuser);                            
            
            return Ok(new 
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }

    }
}