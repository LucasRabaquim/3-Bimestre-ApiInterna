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
using Microsoft.AspNetCore.Http.HttpResults;

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
            return (users == null) ? UserNotFound() : Ok(users);
        }

        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User loggingUser)
        {
            User registeredUser = await _userService.GetByEmail(loggingUser.Email);

            if (registeredUser == null)
                EmailNotFound();

            if (loggingUser.Password != registeredUser.Password)
                return Unauthorized("Senha incorreta");

            var token = TokenService.GenerateToken(registeredUser);
            return Ok(new {user = registeredUser,token});
        }

        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUser(string email)
        {
            User user = await _userService.GetByEmail(email);

            return (user == null) ? EmailNotFound() : Ok(user);
        }

        [HttpPut("alter")]
        public async Task<IActionResult> PutUser([FromHeader] string token, [FromBody] User user)
        {
            int id = TokenService.DecodeToken(token);

            if (!_userService.UserExists(id))
                return NotFound("Esse usuário não existe");

            if (id != user.UserId)
                return BadRequest("A sessão difere do usuário do qual está tentando alterar");

            bool success = await _userService.UpdateUser(user);
            if(success)
                return Ok($"{user.NameUser} foi alterado");
            else
                return StatusCode(StatusCodes.Status501NotImplemented,
                    "Erro interno, o usuário não pode ser alterado"); 
        }

        [HttpPost("register")]
        public async Task<ActionResult<dynamic>> PostUser([FromBody] User newUser)
        { 
            User user = await _userService.GetByEmail(newUser.Email);
            if(user != null)
                return BadRequest("Já existe um usuário com esse email");

            await _userService.RegisterUser(newUser);
            var token = TokenService.GenerateToken(newUser);
            return Created("Usuário criado",new {user = newUser,token});
        }

        [HttpDelete("deactivate")]
        public async Task<IActionResult> DeactivateUser([FromHeader] string token)
        {
            int id = TokenService.DecodeToken(token);

            User? user = await _userService.GetById(id);
            if (user == null)
                return NotFound("Esse usuário não foi encontrado");

            bool sucess = await _userService.DeactivateUser(id);
            if(sucess)
                return Ok($"{user.NameUser} foi desativado");
            else
                return StatusCode(StatusCodes.Status501NotImplemented,
                    "Erro interno, o usuário não pode ser deletado"); 
        }

        [HttpPost("follow/{email}")]
        public async Task<IActionResult> FollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);
            User? user = await _userService.GetById(id);
            if (user == null)
                return BadRequest("Tente logar novamente antes de tentar denovo");
            
            User followingEmail = await _userService.GetByEmail(email);

            if (followingEmail == null)
                return EmailNotFound();

            await _userService.FollowUser(new FollowUser(user.UserId,email));
            return Ok($"Agora você está seguindo {email}");
        }

        [HttpPost("unfollow/{email}")]
        public async Task<IActionResult> UnfollowUser([FromHeader] string token,string email)
        {
            int id = TokenService.DecodeToken(token);

            User? user = await _userService.GetById(id);
            if (user == null)
                return BadRequest("Tente logar novamente antes de tentar denovo ");
            
            User followingEmail = await _userService.GetByEmail(email);
            if (followingEmail == null)
                return EmailNotFound();

            bool success = await _userService.UnfollowUser(user, followingEmail);
            if(success)
                return Ok($"Você não está mais seguindo {email}");
            else
                return BadRequest($"Você não estava seguindo {email} antes");            
        }


        [HttpGet("followingList/{email}")]
        public async Task<ActionResult<IEnumerable<User>>> FollowingUser(string email)
        {
            User user = await _userService.GetByEmail(email);
            if (user == null)
                return UserNotFound();

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
            User user = await _userService.GetByEmail(email);
            if (user == null)
                return UserNotFound();

            var followingUsers = await _userService.GetFollowerList(email);

            if(followingUsers == null)
                return NotFound($"{user.NameUser} não tem seguidores");

            List<User> list = new(){};
            foreach(int i in followingUsers)
                list.Add(await _userService.GetById(i));
        
            return list;
        }

        public ActionResult EmailNotFound() => NotFound("Não foi encontrado um usuário com esse email");
        public ActionResult UserNotFound() => NotFound("O usuário não foi encontrado");
        
    }
}
