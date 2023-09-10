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
        public readonly IUserService _userService;
        public readonly IPostService _postService;
        public readonly MsgActionResultService _msgService;
        public PostsController(IUserService userService, IPostService postService, MsgActionResultService msgService){
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

            await _postService.UpdatePost(updatePost);
            return Ok("O post foi atualizado");
        }

    
        [HttpPost]
        public async Task<ActionResult<Post>> PostPost([FromHeader] string token, Post post)
        {
            int userId = TokenService.DecodeToken(token);
            if (userId != post.UserId)
                return _msgService.MsgInvalid();
                
            await _postService.CreatePost(post);
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

            await _postService.DeletePost(post);

            return Ok("O post foi deletado");
        }
    }
}
