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
using MySqlX.XDevAPI.Common;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;

        public readonly MsgActionResultService _msgService;
        public UserController(IUserService userService, MsgActionResultService msgService){
            _userService = userService;
            _msgService = msgService;
        }
        

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            List<User>? users = await _userService.GetAll();
            if(users == null)
                return _msgService.MsgUserNotFound();
            else
                return users;
        }

        [HttpGet("debug")]
        public async Task<IActionResult> Debug()
        {
            return Ok("Teste");
        }

        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User loggingUser)
        {
            User? registeredUser = await _userService.GetByEmail(loggingUser.Email);

            if (registeredUser == null)
                return _msgService.MsgUserNotFound();

            if (loggingUser.Password != registeredUser.Password)
                return _msgService.MsgWrongPassword();

            var token = TokenService.GenerateToken(registeredUser);
            return new { user = registeredUser,token };
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            User? user = await _userService.GetByEmail(email);
            if (user == null)
                return _msgService.MsgUserNotFound();
            return user;
        }

        [HttpPut("alter")]
        public async Task<IActionResult> PutUser([FromHeader] string token, [FromBody] User user)
        {
            int id = TokenService.DecodeToken(token);

            if (!_userService.UserExists(id))
                return _msgService.MsgUserNotFound();

            if (id != user.UserId)
                return _msgService.MsgInvalid();

            bool success = await _userService.UpdateUser(user);
            if(success)
                return Ok($"{user.NameUser} Foi alterado");
            else
                return _msgService.MsgInternalError("usuário","alteração");
        }

        [HttpPost("register")]
        public async Task<ActionResult<dynamic>> PostUser([FromBody] User newUser)
        { 
            User? user = await _userService.GetByEmail(newUser.Email);
            if(user != null)
                return _msgService.MsgAlreadyExists();

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
                return _msgService.MsgUserNotFound();

            bool sucess = await _userService.DeactivateUser(id);
            if(sucess)
                return Ok($"{user.NameUser} foi desativado");
            else
                return _msgService.MsgInternalError("usuario","desativação");
        }    
    }
}