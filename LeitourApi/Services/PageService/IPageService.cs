using LeitourApi.Models;
namespace LeitourApi.Services.PageService;

public interface IPageService
{
   public Task<Page?> GetById(int id);

   public Task<List<int>?>  GetPageFollowers(int id);

   public Task<List<Page?>> GetPageList(int id);
}