using API.Data;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.DTOs;
using System.Linq;
using AutoMapper;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {


        private readonly DataContext _context;

        private readonly ITokenService _tokenService;

        public string Username { get; private set; }
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)

        {
            _mapper = mapper;
            _tokenService = tokenService;
            _context = context;
        }



        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(DTOs.RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken");

            var user= _mapper.Map<AppUser>(registerDto);


            HMACSHA512 hmac = new HMACSHA512();
            

    

                user.UserName = registerDto.Username.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt = hmac.Key;
        
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return  new UserDto

            {
                Username = user.UserName, 
                Token= _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(DTOs.LoginDto loginDto)

        {
            var user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

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
                Token= _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs

            };
        }
        private async Task<bool> UserExists(string Username)
        {
            return await _context.Users.AnyAsync(ECKeyXmlFormat => ECKeyXmlFormat.UserName == Username.ToLower());
        }
    }
}