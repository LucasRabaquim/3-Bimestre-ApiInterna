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
            
            var newFollowing = await _context.Users.Where(user => user.Email == email)
                    .FirstOrDefaultAsync();

            if (newFollowing == null)
                return NotFound();

            _context.FollowingLists.Add(new FollowingList(user.UserId));
            await _context.SaveChangesAsync();

            var followingList = await _context.FollowingLists.Where(list => list.UserId == user.UserId)
                    .FirstOrDefaultAsync();

            if(followingList == null)
                return NotFound();

            _context.FollowingUsers.Add(new FollowingUser(followingList.FollowingListId,newFollowing.UserId) );
            await _context.SaveChangesAsync();

            return Ok($"You are now following {email}");
        }


        [HttpGet("followingList/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> FollowingUser(string email)
        {
            var user = await _context.Users.Where(user => user.Email == email)
                    .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            var followingList = await _context.FollowingLists.Where(list => list.UserId == user.UserId)
                .FirstOrDefaultAsync();

            if(followingList == null)
                return NotFound($"{user.NameUser} is following no one");

            try{
                var followingUser = await _context.FollowingUsers.Where(flUser => flUser.FollowingListId == followingList.FollowingListId)
                    .ToListAsync();
               
                return users;
            }
            catch{
                return NotFound($"{user.NameUser} is following no one");
            }
        }
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
