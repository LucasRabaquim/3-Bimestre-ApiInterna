using LeitourApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.UserService;
public class UserService : IUserService
{
    private readonly LeitourContext _context;
    public UserService(LeitourContext context) => _context = context;

    public async Task<List<User?>> GetAll()
    {
        if (_context.Users == null)
            return null;
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetById(int id){
        if (_context.Users == null)
                return null;    
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        if (_context.Users == null)
            return null;
        return await _context.Users.Where(user => user.Email == email).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateUser(User user)
    {
        bool success;
        _context.Entry(user).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            success = true;
        }
        catch (DbUpdateConcurrencyException)
        {
            success = false;
        }
        return success;
    }

    public bool UserExists(int id)
    {
        return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
    }

    public async Task RegisterUser(User user){
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

    }

    public async Task<bool> DeactivateUser(int id){
        User? user = await GetById(id);
        user.ActiveUser = false;
        return await UpdateUser(user);
    }

    public async Task FollowUser(FollowUser followUser){
        _context.FollowUsers.Add(followUser);
        await _context.SaveChangesAsync();
        _context.FollowUsers.Add(followUser);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UnfollowUser(User follower, User following){
        FollowUser? follow = await _context.FollowUsers.Where(follow => (follower.UserId == follow.UserId)
                && (following.Email == follow.FollowingEmail)).FirstOrDefaultAsync();
        if(follow == null)
            return false;
        _context.FollowUsers.Remove(follow);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string[]> GetFollowingList(int id){
        return await _context.FollowUsers.Where(list => list.UserId == id)
            .Select(x => x.FollowingEmail).ToArrayAsync();
    }

    public async Task<int[]> GetFollowerList(string email){
        return await _context.FollowUsers.Where(list => list.FollowingEmail == email)
            .Select(x => x.UserId).ToArrayAsync();
    }
}
