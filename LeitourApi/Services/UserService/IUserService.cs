using LeitourApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeitourApi.Services.UserService;

public interface IUserService
{
    Task<List<User>?> GetAll();

    Task<User?> GetByEmail(string email);

    public bool UserExists(int id);

    public Task<bool> UpdateUser(User user);

    public Task RegisterUser(User user);

    public Task<User?> GetById(int id);

    public Task<bool> DeactivateUser(int id);
}