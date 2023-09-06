using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;
using Microsoft.AspNetCore.Http;
using LeitourApi.Services.UserService;
using LeitourApi.Services.MsgActionResult;
using Microsoft.AspNetCore.Http.HttpResults;
using LeitourApi.Services.FollowService;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        public readonly IFollowService _followService;
        public readonly IUserService _userService;

        public readonly MsgActionResultService _msgService;
        public FollowController(IFollowService followService, IUserService userService,MsgActionResultService msgService){
            _followService = followService;
            _msgService = msgService;
            _userService = userService;
        }
        
        [HttpPost("follow/{email}")]
        public async Task<IActionResult> FollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            User? followingEmail = await _userService.GetByEmail(email);

            if (followingEmail == null)
                return _msgService.MsgUserNotFound();

            await _followService.FollowUser(new FollowUser(user.UserId,email));
            return Ok($"Você está seguindo {email}");
        }

        [HttpPost("unfollow/{email}")]
        public async Task<IActionResult> UnfollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);

            User? user = await _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            User? followingEmail = await _userService.GetByEmail(email);
            if (followingEmail == null)
                return NotFound();

            bool success = await _followService.UnfollowUser(user, followingEmail);
            if(success)
                return Ok($"Você não está mais seguindo {email}");
            else
                return BadRequest($"Você não estava seguindo {email} antes");            
        }


        [HttpGet("followingList/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> FollowingUser(string email)
        {
            User? user = await _userService.GetByEmail(email);
            if (user == null)
                return _msgService.MsgUserNotFound();

            var followingUsers = await _followService.GetFollowingList(user.UserId);

            if(followingUsers == null)
                return NotFound($"{user.NameUser} não está seguindo ninguém");

            List<User> list = new(){};
            foreach(string i in followingUsers)
                list.Add(await _userService.GetByEmail(i));
        
            return list;
        }

        [HttpGet("followerList/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> FollowersUser(string email)
        {
            User? user = await _userService.GetByEmail(email);
            if (user == null)
                return NotFound();

            var followingUsers = await _followService.GetFollowerList(email);

            if(followingUsers == null)
                return NotFound($"{user.NameUser} não tem seguidores");

            List<User> list = new(){};
            foreach(int i in followingUsers)
                list.Add(await _userService.GetById(i));
        
            return list;
        }
    }
}