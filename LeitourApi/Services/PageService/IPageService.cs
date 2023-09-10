using LeitourApi.Models;
namespace LeitourApi.Services.PageService;

public interface IPageService
{

   public Task<List<Page>?> GetAllPages();

   public Task<List<Page>?> GetPagesByName(string namePage);
   public Task<Page?> GetById(int id);

   public Task<Page> CreatePage(Page page, int id);

   public Task<bool> VerifyAdmin(int pageId, int id);

   public Task FollowPage(int userId, int pageId);
   public Task UnfollowPage(int userId, int pageId);

   public Task UpdatePage(Page page);

   public Task<bool> VerifyFollowing(int userId, int pageId);

   public Task<List<int>?>  GetPageFollowers(int id);

   public Task<List<Page>?> GetPageList(int id);
}