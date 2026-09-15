using EmpLMS.Models.ViewModels;

namespace EmpLMS.Data.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginUserDto?> ValidateUserLoginAsync(string email);
    }
}
