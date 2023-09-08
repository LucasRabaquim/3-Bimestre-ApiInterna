using LeitourApi.Models;
namespace LeitourApi.Services.PostService;

public interface IPostService{
    public Task<List<Post>?> GetPosts();

    public Task<Post?> GetById(int id);

    public Task<List<Post>?> GetByUserId(int id);

    public Task UpdatePost(Post post);
    public Task CreatePost(Post post);
    public Task DeletePost(Post post);
}