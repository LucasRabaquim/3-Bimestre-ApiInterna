using LeitourApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.PostService;
public class PostService : IPostService
{
    private readonly LeitourContext _context;
    public PostService(LeitourContext context) => _context = context;
    

    public async Task<List<Post>?> GetPosts() => (_context.Posts == null) 
        ? null : await _context.Posts.ToListAsync();

    public async Task<Post?> GetById(int id) =>    
        (_context.Posts == null) ? null :  await _context.Posts.FindAsync(id);


    public async Task<List<Post>?> GetByUserId(int id) => (_context.Posts == null) 
        ? null : await _context.Posts.Where(posts => posts.UserId == id).ToListAsync();

    public async Task UpdatePost(Post post){
        post.AlteratedDate = DateTime.Now;
        _context.Entry(post).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task CreatePost(Post post) {
        post.PostId = 0;
        await _context.Posts.AddAsync(post);
        await _context.SaveChangesAsync();
    }

    public async Task DeletePost(Post post) {
        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
    }
    
}