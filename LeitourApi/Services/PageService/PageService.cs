using LeitourApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.PageService;
public class PageService : IPageService
{
    private readonly LeitourContext _context;
    public PageService(LeitourContext context) => _context = context;

    public async Task<List<Page>?> GetAllPages() => 
        (_context.Pages == null) ? null : await _context.Pages.ToListAsync();

    public async Task<List<Page>?> GetPagesByName(string namePage) => 
        (_context.Pages == null) ? null : await _context.Pages.Where(page => page.NamePage == namePage).ToListAsync();
    
    public async Task<Page?> GetById(int id) =>    
        (_context.Pages == null) ? null :  await _context.Pages.FindAsync(id);
    
    public async Task<Page> CreatePage(Page page, int id) {
        _context.Pages.Add(page);
        await _context.SaveChangesAsync();
        FollowingPage flPage = new(id, page.PageId, (int) RolePage.Creator);
        _context.FollowingPages.Add(flPage);
        await _context.SaveChangesAsync();
        return page;
    }

    public async Task<bool> VerifyAdmin(int pageId, int id){
        var adminUser = await _context.FollowingPages.Where(flPage => flPage.UserId == id
                && flPage.PageId == pageId
                && flPage.RoleUser != (int)RolePage.Common).FirstOrDefaultAsync();
        return adminUser == null;
    }

    public async void UpdatePage(Page page){
        page.AlteratedDate = DateTime.Now;
        _context.Entry(page).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async void FollowPage(int userId, int pageId){
        FollowingPage flPage = new(userId, pageId, (int) RolePage.Common);
        _context.FollowingPages.Add(flPage);
        await _context.SaveChangesAsync();
    }

    public async void UnfollowPage(int userId, int pageId){
        FollowingPage flPage = new(userId, pageId, (int) RolePage.Common);
        _context.FollowingPages.Remove(flPage);
        await _context.SaveChangesAsync();
    }

    public async Task<List<int>?> GetPageFollowers(int id)
    {
        var flPage = await _context.FollowingPages.Where(flPage => flPage.PageId == id)
            .Select(page => page.UserId).ToListAsync();
        return flPage;
    }

    public async Task<List<Page?>> GetPageList(int id)
    {
        var flPage = await _context.FollowingPages.Where(flPage => flPage.UserId == id)
            .Select(page => page.PageId).ToListAsync();
        if (flPage == null)
            return null;
        List<Page> list = new() { };
        foreach (int i in flPage)
            list.Add(await GetById(i));
        return list;
    }

    public async Task<bool> VerifyFollowing(int userId, int pageId){
        var flPage = await _context.FollowingPages.
            Where(flPage => flPage.UserId == userId && flPage.PageId == pageId).
            FirstOrDefaultAsync();
        return flPage  != null;
    }
}