using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ExpenseTrackerApi.Data;
using ExpenseTrackerApi.Interfaces;
using ExpenseTrackerApi.Models;

namespace ExpenseTrackerApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;

        public UserService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Get all user
            return await _db.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            // Get user by Id
            return await _db.Users.FindAsync(id);
        }
    }
}