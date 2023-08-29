using LeitourApi.Models;
namespace LeitourApi.Services.UserService;

public interface IUserService
{
    Task<List<User>> GetAll();

    Task<User> GetByEmail(string email);

    public bool UserExists(int id);

    public Task<bool> UpdateUser(User user);

    public Task RegisterUser(User user);

    public Task<User?> GetById(int id);

    public Task<bool> DeactivateUser(int id);

    public Task FollowUser(FollowUser flUser);

    public Task<bool> UnfollowUser(User follower, User following);

    public Task<string[]> GetFollowingList(int id);
    public Task<int[]> GetFollowerList(string email);
}