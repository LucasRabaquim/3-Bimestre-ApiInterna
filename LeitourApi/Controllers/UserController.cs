using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;
using LeitourApi.Services.UserService;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        public UserController(IUserService userService) => _userService = userService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            List<User> users = await _userService.GetAll();
            if(users == null)
                return NotFound();
            else
                return users;
        }


        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User loggingUser)
        {
            User registeredUser = await _userService.GetByEmail(loggingUser.Email);

            if (registeredUser == null)
                return NotFound("User does not exist");

            if (loggingUser.Password != registeredUser.Password)
                return NoContent();

            var token = TokenService.GenerateToken(registeredUser);
            return new { user = registeredUser,token };
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            User user = await _userService.GetByEmail(email);

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpGet("DEBUG/{token}")]
        public async Task<ActionResult<int>> DEBUGID(string token)
        {
            return TokenService.DecodeToken(token);
        }

        [HttpPut("alter")]
        public async Task<IActionResult> PutUser([FromHeader] string token, [FromBody] User user)
        {
            int id = TokenService.DecodeToken(token);

            if (!_userService.UserExists(id))
                return NotFound("This user doesn't exist");

            if (id != user.UserId)
                return BadRequest("Operation is not Valid");

            bool success = await _userService.UpdateUser(user);
            if(success)
                return Ok($"{user.NameUser} Has been alterated");
            else
                return BadRequest("The user couldn't be alterated"); 
        }

        [HttpPost("register")]
        public async Task<ActionResult<dynamic>> PostUser([FromBody] User newUser)
        { 
            User user = await _userService.GetByEmail(newUser.Email);
            if(user != null)
                return BadRequest("User with this email already exists");

            await _userService.RegisterUser(newUser);
            var token = TokenService.GenerateToken(newUser);
            return new { id = newUser.UserId, token };
        }

        [HttpDelete("deactivate")]
        public async Task<IActionResult> DeactivateUser([FromHeader] string token)
        {
            int id = TokenService.DecodeToken(token);

            User? user = await _userService.GetById(id);
            if (user == null)
                return NotFound();

            bool sucess = await _userService.DeactivateUser(id);
            if(sucess)
                return Ok($"{user.NameUser} Has been deactivated");
            else
                return BadRequest("The user couldn't be deleted");
        }

        [HttpPost("follow/{email}")]
        public async Task<IActionResult> FollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            User followingEmail = await _userService.GetByEmail(email);

            if (followingEmail == null)
                return NotFound($"user with {email} does not exists");

            await _userService.FollowUser(new FollowUser(user.UserId,email));
            return Ok($"You are now following {email}");
        }

        [HttpPost("unfollow/{email}")]
        public async Task<IActionResult> UnfollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);

            User? user = await _userService.GetById(id);
            if (user == null)
                return NotFound();
            
            User followingEmail = await _userService.GetByEmail(email);
            if (followingEmail == null)
                return NotFound();

            bool success = await _userService.UnfollowUser(user, followingEmail);
            if(success)
                return Ok($"You are not following {email} anymore");
            else
                return NotFound($"You was not following {email} before");            
        }


        [HttpGet("followingList/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> FollowingUser(string email)
        {
            User user = await _userService.GetByEmail(email);
            if (user == null)
                return NotFound();

            var followingUsers = await _userService.GetFollowingList(user.UserId);

            if(followingUsers == null)
                return NotFound($"{user.NameUser} is following no one");

            List<User> list = new(){};
            foreach(string i in followingUsers)
                list.Add(await _userService.GetByEmail(i));
        
            return list;
        }
    }
}
