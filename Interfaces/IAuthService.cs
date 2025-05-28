using ExpenseTrackerApi.DTOs.Auth;
namespace ExpenseTrackerApi.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }
}
