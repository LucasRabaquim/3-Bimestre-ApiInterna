using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;
using NuGet.Common;
using LeitourApi.Services.UserService;
using LeitourApi.Services.PageService;
using LeitourApi.Services.MsgActionResult;

using Microsoft.AspNetCore.Http.HttpResults;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly LeitourContext _context;
        public readonly IUserService _userService;
        public readonly IPageService _pageService;

        public readonly MsgActionResultService _msgService;

        public PageController(LeitourContext context, IUserService userService, IPageService pageService, MsgActionResultService msgService)
        {
            _context = context;
            _userService = userService;
            _pageService = pageService;
             _msgService = msgService;
        }
        


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Page>>> GetPages()
        {
            List<Page>? pages = await _pageService.GetAllPages();
            return (pages == null) ? NotFound("Não foi encontrada nenhuma página"): pages;
        }

        [HttpGet("name/{namePage}")]
        public async Task<ActionResult<IEnumerable<Page>>> GetPageByName(string namePage)
        {
            List<Page>? pages = await _pageService.GetPagesByName(namePage);
            return (pages == null) ? NotFound("Não foi encontrada nenhuma página com esse nome"): pages;
        }

        [HttpGet("{PageId}")]
        public async Task<ActionResult<Page>> GetPage(int PageId)
        {
            Page? page = await _pageService.GetById(PageId);
            return (page == null) ? _msgService.MsgPageNotFound() : Ok(page);
        }

        [HttpPost("create")]
        public async Task<ActionResult<dynamic>> PostPage([FromHeader] string token, [FromBody] Page Page)
        {
            int id = TokenService.DecodeToken(token);
            if(_userService.GetById(id) == null)
                _msgService.MsgInvalid();

            Page = await _pageService.CreatePage(Page,id);

            return new { Page, message = "A página foi criada" };
        }

        [HttpPut("alter/{PageId}")]
        public async Task<IActionResult> PutPage(int PageId, [FromHeader] string token, [FromBody] Page NewPage)
        {
            int id = TokenService.DecodeToken(token);
            if(_userService.GetById(id) == null)
                _msgService.MsgInvalid();

            var page = await _pageService.GetById(PageId);

            if (page == null)
                return _msgService.MsgPageNotFound();

            var adminUser = await _pageService.VerifyAdmin(PageId,id);

            if (adminUser)
                return BadRequest("Você não é da admistração dessa página");

            _pageService.UpdatePage(NewPage);

            return Ok($"{NewPage.NamePage} foi alterada");
        }

        [HttpDelete("deactivate/{PageId}")]
        public async Task<IActionResult> DeactivatePage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return _msgService.MsgPageNotFound();

            var adminUser = await _context.FollowingPages.Where(flPage => flPage.UserId == id
                && flPage.PageId == PageId
                && flPage.RoleUser == (int)RolePage.Creator).FirstOrDefaultAsync();

            if (adminUser == null)
                return BadRequest("Você não criou essa página");

            Page.ActivePage = false;
            _context.Entry(Page).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok($"{Page.NamePage} foi desativada");
        }

        [HttpPost("follow/{pageId}")]
        public async Task<IActionResult> FollowPage([FromHeader] string token, int pageId)
        {
            int id = TokenService.DecodeToken(token);
            if(_userService.GetById(id) == null)
                _msgService.MsgInvalid();

            var Page = await _pageService.GetById(pageId);
            if (Page == null)
                return _msgService.MsgPageNotFound();

            bool alreadyFollowing = await _pageService.VerifyFollowing(id, pageId);

            if(alreadyFollowing)
                return BadRequest("Você já está seguindo esta página");
            
            await _pageService.FollowPage(id, pageId);
            return Ok($"Você está seguindo {Page.NamePage}");
        }

        [HttpPost("unfollow/{pageId}")]
        public async Task<IActionResult> UnfollowPage([FromHeader] string token, int pageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(pageId);
            if (Page == null)
                return _msgService.MsgPageNotFound();
              
            bool alreadyFollowing = await _pageService.VerifyFollowing(id, pageId);
            
            if(!alreadyFollowing)
                return NotFound($"Você não estava seguindo {Page.NamePage} antes");

            _pageService.UnfollowPage(id, pageId);
            return Ok($"Você não está mais seguindo {Page.NamePage}");
        }


        [HttpGet("followingPages")]
        public async Task<ActionResult<IEnumerable<Page?>>> FollowingPage([FromHeader] string token)
        {
            int id = TokenService.DecodeToken(token);
            var User = await _userService.GetById(id);
            if (User == null)
                return _msgService.MsgUserNotFound();

            List<Page?> followingPages = await _pageService.GetPageList(id);

            if (followingPages == null)
                return NotFound($"Você não segue nenhuma página");

            return followingPages;
        }

        [HttpGet("PageFollowers/{PageId}")]
        public async Task<ActionResult<IEnumerable<User>>> PageFollowers(int PageId)
        {
            var Page = await _pageService.GetById(PageId);
            if (Page == null)
                return _msgService.MsgPageNotFound();

            var pageFollowers = await _pageService.GetPageFollowers(PageId);

            if (pageFollowers == null)
                return NotFound($"Essa pagina não tem seguidores");

            List<User> list = new(){};
            foreach(int i in pageFollowers)
                list.Add(await _userService.GetById(i));
            return Ok(list);
        }

       /*[ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult _msgService.MsgUserNotFound() => NotFound("O usuário não existe.");
        [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult _msgService.MsgInvalid() => BadRequest("Autenticação invalida, logue novamente.");
         [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult _msgService.MsgInternalError(string obj,string acao) => StatusCode(StatusCodes.Status500InternalServerError, $"A {acao} de {obj} não foi bem sucedida.");  
         [ApiExplorerSettings(IgnoreApi = true)]
        public ActionResult _msgService.MsgPageNotFound() => NotFound("A página não foi encontrada");*/
    }
}