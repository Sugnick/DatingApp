using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            //Validate Request Placeholder

            //Providing the flexibility to user to be able to register with Upper Case and Lower Case Name
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            //Check the userName exists
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("User already exists");

            //Create the Json Object of the Model User to store the userName
            var userToCreate = new User
            {
                UserName = userForRegisterDto.Username
            };

            //Now Call the register method
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) {

            // Get the User from db
            var userFromRepo = await _repo.Login(userForLoginDto.UserName.ToLower(), userForLoginDto.Password);

            //Now Check the User Credentials are correct or not
            if(userFromRepo == null)
            return Unauthorized();

            //Create Token and save it
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            //Now We Need Signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //Now give the Token Descriptor
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //Create a Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //Create a Token to return to the user
            var token = tokenHandler.CreateToken(tokenDescriptor);

            //Return the Token as an object    
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}