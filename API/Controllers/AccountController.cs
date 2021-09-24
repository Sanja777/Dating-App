using API.Data;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.DTOs;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {


        private readonly DataContext _context;

        private readonly ITokenService _tokenService;

        public string Username { get; private set; }

        public AccountController(DataContext context, ITokenService tokenService)

        {
            _tokenService = tokenService;
            _context = context;
        }



        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(dtoS.RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");


            HMACSHA512 hmac = new HMACSHA512();
            var user = new AppUser
            {

                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };



            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return  new UserDto

            {
                Username = user.UserName, 
                Token= _tokenService.CreateToken(user)
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(DTOs.LoginDto loginDto)

        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) return Unauthorized("Invalid username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < ComputeHash.Length; i++)
            {
                if (ComputeHash[i] != user.PasswordHash[i]) return Unauthorized("invalid password");
            }


             return  new UserDto

            {
                Username = user.UserName, 
                Token= _tokenService.CreateToken(user)
            };
        }
        private async Task<bool> UserExists(string Username)
        {
            return await _context.Users.AnyAsync(ECKeyXmlFormat => ECKeyXmlFormat.UserName == Username.ToLower());
        }
    }
}