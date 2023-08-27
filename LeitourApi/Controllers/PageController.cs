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
            return await _context.Pages.ToListAsync();
        }

        [HttpGet("{PageId}")]
        public async Task<ActionResult<Page>> GetPage(int PageId)
        {

            if (_context.Pages == null)
                return NotFound();

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return NotFound();

            return Page;
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

            return new { Page, message = "Page has been created" };
        }

        [HttpPut("alter/{PageId}")]
        public async Task<IActionResult> PutPage(int PageId, [FromHeader] string token, [FromBody] Page NewPage)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return NotFound();

            var adminUser = await _context.FollowingPages.Where(flPage => flPage.UserId == id
                && flPage.PageId == PageId
                && flPage.RoleUser != (int)RolePage.Common).FirstOrDefaultAsync();

            if (adminUser == null)
                return BadRequest("Operation is not Valid");

            NewPage.AlteratedDate = DateTime.Now;

            _context.Entry(Page).State = EntityState.Modified;

           
                await _context.SaveChangesAsync();
            

            return Ok($"{Page.NamePage} Has been alterated");
        }



        [HttpDelete("deactivate/{PageId}")]
        public async Task<IActionResult> DeactivatePage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);

            if (Page == null)
                return NotFound();

            var adminUser = await _context.FollowingPages.Where(flPage => flPage.UserId == id
                && flPage.PageId == PageId
                && flPage.RoleUser == (int)RolePage.Creator).FirstOrDefaultAsync();

            if (adminUser == null)
                return BadRequest("Operation is not Valid");

            Page.ActivePage = false;
            _context.Entry(Page).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok($"{Page.NamePage} Has been deactivated");
        }

        [HttpPost("follow/{PageId}")]
        public async Task<IActionResult> FollowPage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);
            if (Page == null)
                return NotFound();

            FollowingPage flPage = new(id, PageId, 0);
            _context.FollowingPages.Add(flPage);
            await _context.SaveChangesAsync();

            return Ok($"You are now following {Page.NamePage}");
        }

        [HttpPost("unfollow/{PageId}")]
        public async Task<IActionResult> UnfollowPage([FromHeader] string token, int PageId)
        {
            int id = TokenService.DecodeToken(token);

            var Page = await _context.Pages.FindAsync(PageId);
            if (Page == null)
                return NotFound();

            var followingPage = await _context.FollowingPages.Where(Page => Page.PageId == PageId
                && Page.UserId == id).FirstOrDefaultAsync();

            if (followingPage == null)
                return NotFound($"You not following {Page.NamePage}");

            _context.FollowingPages.Remove(followingPage);
            await _context.SaveChangesAsync();

            return Ok($"You are not following {Page.NamePage} anymore");
        }


        [HttpGet("followingPages")]
        public async Task<ActionResult<IEnumerable<Page>>> FollowingPage([FromHeader] string token)
        {
            int id = TokenService.DecodeToken(token);
            var User = await _userService.GetById(id);
            if (User == null)
                return NotFound();

            List<Page> followingPages = await _pageService.GetPageList(id);

            if (followingPages == null)
                return NotFound($"You are not following any page");

            return followingPages;
        }

        [HttpGet("PageFollowers/{PageId}")]
        public async Task<ActionResult<IEnumerable<User>>> PageFollowers(int PageId)
        {
            var Page = await _pageService.GetById(PageId);
            if (Page == null)
                return NotFound();

            var pageFollowers = await _pageService.GetPageFollowers(PageId);

            if (pageFollowers == null)
                return NotFound($"This page has no followers");

            List<User> list = new(){};
            foreach(int i in pageFollowers)
                list.Add(await _userService.GetById(i));
            return list;
        }

    }
}