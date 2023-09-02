using Microsoft.AspNetCore.Mvc;
using LeitourApi.Models;
using LeitourApi.Services;
using LeitourApi.Services.UserService;
using LeitourApi.Services.PostService;
using LeitourApi.Services.MsgActionResult;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly LeitourContext _context;

        public readonly IUserService _userService;
        public readonly IPostService _postService;
        public readonly MsgActionResultService _msgService;
        public PostsController(LeitourContext context, IUserService userService, IPostService postService, MsgActionResultService msgService){
            _context = context;
            _userService = userService;
            _postService = postService;
            _msgService = msgService;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _postService.GetPosts();
            return (posts == null) ? _msgService.MsgPostNotFound() : posts;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postService.GetById(id);
            return (post == null) ? _msgService.MsgPostNotFound() : post;
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPost(string email)
        {
            var user = await _userService.GetByEmail(email);
            if(user == null)
                return _msgService.MsgUserNotFound();

            var posts = await _postService.GetByUserId(user.UserId);

            if (posts == null)
                return _msgService.MsgPostNotFound();

            return posts;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPost([FromHeader]string token, int id, [FromBody] Post updatePost)
        {
            var post = await _postService.GetById(id);
            if (post == null)
                return _msgService.MsgPostNotFound();

            if (id != updatePost.PostId)
                return _msgService.MsgInvalidPost();

            int userId = TokenService.DecodeToken(token);

            if (userId != updatePost.UserId)
                return _msgService.MsgInvalid();

            _postService.UpdatePost(updatePost);
            return Ok("O post foi atualizado");
        }

    
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromHeader] string token, Post post)
        {
            int userId = TokenService.DecodeToken(token);
            if (userId != post.UserId)
                return _msgService.MsgInvalid();
                
            _postService.CreatePost(post);
            return CreatedAtAction("GetPost", new { id = post.PostId }, post);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost([FromHeader] string token, int id)
        {
            var post = await _postService.GetById(id);
            if (post == null)
                return _msgService.MsgPostNotFound();

            int userId = TokenService.DecodeToken(token);

            if (userId != post.UserId)
                return _msgService.MsgInvalidPost();

            _postService.DeletePost(post);

            return Ok("O post foi deletado");
        }
/*
        public ActionResult _msgService.MsgUserNotFound() => NotFound("O usuário não existe.");
        public ActionResult _msgService.MsgPostNotFound() => NotFound("O post não foi encontrado");
        public ActionResult _msgService.MsgInvalid() => BadRequest("Autenticação invalida, logue novamente.");
        public ActionResult _msgService.MsgInvalidPost() => BadRequest("Algo deu errado na atualização do post");
        public ActionResult _msgService.MsgAlreadyExists() => BadRequest("Já existe usuário com esse email.");
        public ObjectResult _msgService.MsgInternalError(string obj,string acao) => StatusCode(StatusCodes.Status500InternalServerError, $"A {acao} de {obj} não foi bem sucedida.");
  
        public ActionResult _msgService.MsgPageNotFound() => NotFound("A página não foi encontrada");*/
    }
}
