using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;
using Org.BouncyCastle.Bcpg;
using Azure.Messaging;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LeitourContext _context;
        public UserController(LeitourContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            if (_context.Users == null)
                return NotFound();
            return await _context.Users.ToListAsync();
        }


        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User loggingUser)
        {
            var registeredUser = await _context.Users.Where(user => user.Email == loggingUser.Email)
                    .FirstOrDefaultAsync();

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

            if (_context.Users == null)
              return NotFound();
          
            var user = await _context.Users.Where(user => user.Email == email)
                    .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpPut("alter")]
        public async Task<IActionResult> PutUser([FromHeader] string token, [FromBody] User user)
        {
            int id = TokenService.DecodeToken(token);

            if (id != user.UserId)
                return BadRequest("Operation is not Valid");

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                    return NotFound();
                else
                    throw;
            }

            return Ok($"{user.NameUser} Has been alterated");
        }

        [HttpPost("register")]
        public async Task<ActionResult<dynamic>> PostUser([FromBody] User user)
        {
          if (_context.Users == null)
              return Problem("Entity set 'LeitourContext.Users'  is null.");
          
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = TokenService.GenerateToken(user);

            return new { id = user.UserId, token };
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromHeader] string token)
        {
            int id = TokenService.DecodeToken(token);
            if (_context.Users == null)
                return NotFound();
            
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok($"{user.NameUser} Has been deleted");
        }

        [HttpPost("follow/{email}")]
        public async Task<IActionResult> FollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);
            if (_context.Users == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            
            var followingEmail = await _context.Users.Where(user => user.Email == email)
                    .FirstOrDefaultAsync();

            if (followingEmail == null)
                return NotFound($"user with {email} does not exists");

            _context.FollowingLists.Add(new FollowingList(user.UserId,email));
            await _context.SaveChangesAsync();

            return Ok($"You are now following {email}");
        }

        [HttpPost("unfollow/{email}")]
        public async Task<IActionResult> UnfollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);
            if (_context.Users == null)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();
            
            var followingEmail = await _context.Users.Where(user => user.Email == email)
                    .FirstOrDefaultAsync();

            if (followingEmail == null)
                return NotFound($"user with {email} does not exists");


            var following = await _context.FollowingLists.Where(following => (id == following.UserId) && (email == following.FollowingEmail))
                    .FirstOrDefaultAsync();
            
            if(following == null)
                return NotFound($"You not following {email}");
            _context.FollowingLists.Remove(following);
            await _context.SaveChangesAsync();

            return Ok($"You are not following {email} anymore");
        }


        [HttpGet("followingList/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> FollowingUser(string email)
        {
            var x = await _context.Users.Where(user => user.Email == email)
                    .FirstOrDefaultAsync();

            if (x == null)
                return NotFound();

            var followingUsers = _context.FollowingLists.Where(list => list.UserId == x.UserId).Select(x => x.FollowingEmail).ToArray();

            if(followingUsers == null)
                return NotFound($"{x.NameUser} is following no one");

            List<User> list = new(){};
            foreach(string i in followingUsers)
                list.Add(await _context.Users.Where(user => user.Email == i)
                    .FirstOrDefaultAsync());
            
            return list;
        }
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
