using LeitourApi.Models;
namespace LeitourApi.Services.PostService;

public interface IPostService{
    public Task<List<Post>?> GetPosts();

    public Task<Post?> GetById(int id);

    public Task<List<Post>?> GetByUserId(int id);

    public void UpdatePost(Post post);
    public void CreatePost(Post post);
    public void DeletePost(Post post);
}