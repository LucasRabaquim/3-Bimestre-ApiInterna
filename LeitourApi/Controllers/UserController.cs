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

            await _userService.FollowUser(new FollowUser(user.UserId,email));
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

            bool success = await _userService.UnfollowUser(user, followingEmail);
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

            var followingUsers = await _userService.GetFollowingList(user.UserId);

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

            var followingUsers = await _userService.GetFollowerList(email);

            if(followingUsers == null)
                return NotFound($"{user.NameUser} não tem seguidores");

            List<User> list = new(){};
            foreach(int i in followingUsers)
                list.Add(await _userService.GetById(i));
        
            return list;
        }

       /* public ActionResult _msgService.MsgUserNotFound() => NotFound("O usuário não existe.");
        public ActionResult _msgService.MsgWrongPassword() => BadRequest("Senha incorreta.");
        public ActionResult _msgService.MsgInvalid() => BadRequest("Autenticação invalida, logue novamente.");
        public ActionResult _msgService.MsgAlreadyExists() => BadRequest("Já existe usuário com esse email.");
        public ObjectResult _msgService.MsgInternalError(string obj,string acao) => StatusCode(StatusCodes.Status500InternalServerError, $"A {acao} de {obj} não foi bem sucedida.");
  */      
    }
}
