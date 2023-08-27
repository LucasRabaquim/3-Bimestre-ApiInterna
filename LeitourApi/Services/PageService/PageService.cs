using LeitourApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.PageService;
public class PageService : IPageService
{
    private readonly LeitourContext _context;
    public PageService(LeitourContext context) => _context = context;
    
     public async Task<Page?> GetById(int id){
        if (_context.Pages == null)
                return null;    
        return await _context.Pages.FindAsync(id);
    }

public async Task<List<int>?> GetPageFollowers(int id){
        var flPage = await _context.FollowingPages.Where(flPage => flPage.PageId == id)
            .Select(page => page.UserId).ToListAsync();
        return flPage;
    }

    public async Task<List<Page?>> GetPageList(int id){
        var flPage = await _context.FollowingPages.Where(flPage => flPage.UserId == id)
            .Select(page => page.PageId).ToListAsync();
        if(flPage == null)
            return null;
        List<Page> list = new(){};
        foreach(int i in flPage)
                list.Add(await GetById(i));
        return list;
    }
}
