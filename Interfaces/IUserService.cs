using ExpenseTrackerApi.Models;
namespace ExpenseTrackerApi.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
       
    }
}
