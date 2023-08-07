using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LeitourContext _context;

        public UserController(LeitourContext context) => _context = context;



        // GET: api/User
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
            var registeredUser = _context.Users.Where(user => user.Email == loggingUser.Email)
                    .FirstOrDefault();

            if (registeredUser == null)
                return NotFound("User does not exist");

            if (loggingUser.Password != registeredUser.Password)
                return NoContent();

            var token = TokenService.GenerateToken(registeredUser);
            return new { user = registeredUser, token = token };
        }


        // GET: api/User/marco@gmail.com
        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {

            if (_context.Users == null)
              return NotFound();
          
            var user = _context.Users.Where(user => user.Email == email)
                    .FirstOrDefault();

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

            return NoContent();
        }

        [HttpPost("register")]
        public async Task<ActionResult<dynamic>> PostUser([FromBody] User user)
        {
          if (_context.Users == null)
              return Problem("Entity set 'LeitourContext.Users'  is null.");
          
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = TokenService.GenerateToken(user);

            return new { id = user.UserId, token = token};
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

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
