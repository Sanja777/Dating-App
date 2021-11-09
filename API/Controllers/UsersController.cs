using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using API.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]

    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;

        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;

        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            /*var users = await _userRepository.GetUsersAsync();

            return Ok(users);*/
            var users = await _userRepository.GetUsersAsync();

            var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);

            return Ok(usersToReturn);
        }


        [HttpGet("{username}")]

        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }


        [HttpPut]
         public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
         {
             var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

             var user = await _userRepository.GetUserByUsernameAsync(username);



             _mapper.Map(memberUpdateDto, user);

             _userRepository.Update(user);

             if (await _userRepository.SaveAllAsync()) return NoContent();
             return BadRequest("Failed to update user");
             
         }
    }
}
