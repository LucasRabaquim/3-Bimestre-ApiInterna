using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeitourApi.Models;
using LeitourApi.Services;
using NuGet.Common;
using LeitourApi.Services;
using LeitourApi.Services.UserService;
using LeitourApi.Services.PageService;

namespace LeitourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly LeitourContext _context;
        public readonly IUserService _userService;
        public readonly IPageService _pageService;

        public PageController(LeitourContext context, IUserService userService, IPageService pageService)
        {
            _context = context;
            _userService = userService;
            _pageService = pageService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Page>>> GetPages()
        {
            if (_context.Pages == null)
                return NotFound();
            return Ok(await _context.Pages.ToListAsync());
        }

        [HttpGet("{PageId}")]
        public async Task<ActionResult<Page>> GetPage(int PageId)
        {

            if (_context.Pages == null)
                return NotFound();

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return PageNotFound();

            return Ok(Page);
        }

        [HttpPost("create")]
        public async Task<ActionResult<dynamic>> PostPage([FromHeader] string token, [FromBody] Page Page)
        {
            int id = TokenService.DecodeToken(token);

            _context.Pages.Add(Page);
            await _context.SaveChangesAsync();

            FollowingPage flPage = new(id, Page.PageId, (int) RolePage.Creator);
            _context.FollowingPages.Add(flPage);
            await _context.SaveChangesAsync();

            return new { Page, message = "A página foi criada" };
        }

        [HttpPut("alter/{PageId}")]
        public async Task<IActionResult> PutPage(int PageId, [FromHeader] string token, [FromBody] Page NewPage)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return PageNotFound();

            var adminUser = await _context.FollowingPages.Where(flPage => flPage.UserId == id
                && flPage.PageId == PageId
                && flPage.RoleUser != (int)RolePage.Common).FirstOrDefaultAsync();

            if (adminUser == null)
                return BadRequest("Você não é da admistração dessa página");

            NewPage.AlteratedDate = DateTime.Now;

            _context.Entry(Page).State = EntityState.Modified;

           
                await _context.SaveChangesAsync();
            

            return Ok($"{Page.NamePage} foi alterada");
        }



        [HttpDelete("deactivate/{PageId}")]
        public async Task<IActionResult> DeactivatePage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return PageNotFound();

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

        [HttpPost("follow/{PageId}")]
        public async Task<IActionResult> FollowPage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);
            if (Page == null)
                return PageNotFound();

            FollowingPage flPage = new(id, PageId, 0);
            _context.FollowingPages.Add(flPage);
            await _context.SaveChangesAsync();

            return Ok($"Você está seguindo {Page.NamePage}");
        }

        [HttpPost("unfollow/{PageId}")]
        public async Task<IActionResult> UnfollowPage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);
            if (Page == null)
                return PageNotFound();

            var followingPage = await _context.FollowingPages.Where(Page => Page.PageId == PageId
                && Page.UserId == id).FirstOrDefaultAsync();

            if (followingPage == null)
                return NotFound($"Você não estava seguindo {Page.NamePage} antes");

            _context.FollowingPages.Remove(followingPage);
            await _context.SaveChangesAsync();

            return Ok($"Você não está mais seguindo {Page.NamePage}");
        }


        [HttpGet("followingPages")]
        public async Task<ActionResult<IEnumerable<Page>>> FollowingPage([FromHeader] string token)
        {
            int id = TokenService.DecodeToken(token);
            var User = await _userService.GetById(id);
            if (User == null)
                return UserNotFound();

            List<Page> followingPages = await _pageService.GetPageList(id);

            if (followingPages == null)
                return NotFound($"Você não segue nenhuma página");

            return followingPages;
        }

        [HttpGet("PageFollowers/{PageId}")]
        public async Task<ActionResult<IEnumerable<User>>> PageFollowers(int PageId)
        {
            var Page = await _pageService.GetById(PageId);
            if (Page == null)
                return PageNotFound();

            var pageFollowers = await _pageService.GetPageFollowers(PageId);

            if (pageFollowers == null)
                return NotFound($"Essa pagina não tem seguidores");

            List<User> list = new(){};
            foreach(int i in pageFollowers)
                list.Add(await _userService.GetById(i));
            return Ok(list);
        }

        public ActionResult EmailNotFound() => NotFound("Não foi encontrado um usuário com esse email");
        public ActionResult UserNotFound() => NotFound("O usuário não foi encontrado");
        public ActionResult PageNotFound() => NotFound("A página não foi encontrada");

    }
}