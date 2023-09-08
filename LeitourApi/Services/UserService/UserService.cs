using LeitourApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace LeitourApi.Services.UserService;
public class UserService : IUserService
{
    private readonly LeitourContext _context;
    public UserService(LeitourContext context) => _context = context;

    public async Task<List<User>?> GetAll()
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
        User user = await GetById(id);
        user.ActiveUser = false;
        return await UpdateUser(user);
    }
}