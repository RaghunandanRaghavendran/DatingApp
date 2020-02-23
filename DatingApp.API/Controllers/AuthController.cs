using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        public AuthController(IAuthRepository authRepo, IConfiguration config)
        {
            _authRepo = authRepo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegister registorDTO)
        {
            registorDTO.UserName = registorDTO.UserName.ToLower();

            if(await _authRepo.UserExists(registorDTO.UserName))
            return BadRequest("Username already taken");

            User userToCreate = new User()
            {
                UserName = registorDTO.UserName,
            };

            User registereduser = await _authRepo.Register(userToCreate,registorDTO.Password);

            //return CreatedAtRoute();  This is required but time being and will comeback later.
            return StatusCode(201);
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
            
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}