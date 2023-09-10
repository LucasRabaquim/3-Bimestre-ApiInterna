using LeitourApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.FollowService;
public class FollowService : IFollowService
{
    private readonly LeitourContext _context;
    public FollowService(LeitourContext context) => _context = context;

    public async Task FollowUser(FollowUser followUser){
        await _context.FollowUsers.AddAsync(followUser);
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
