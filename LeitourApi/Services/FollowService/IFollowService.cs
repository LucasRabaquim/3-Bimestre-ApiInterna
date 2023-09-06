using LeitourApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeitourApi.Services.FollowService;

public interface IFollowService
{
    public Task FollowUser(FollowUser flUser);
    public Task<bool> UnfollowUser(User follower, User following);
    public Task<string[]> GetFollowingList(int id);
    public Task<int[]> GetFollowerList(string email);
}