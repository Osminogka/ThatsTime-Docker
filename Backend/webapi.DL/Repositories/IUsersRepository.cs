using Microsoft.AspNetCore.Identity;

namespace webapi.DL.Repositories
{
    public interface IUsersRepository
    {
        IQueryable<IdentityUser> Get();
        Task<IdentityUser> GetByUsername(string email);
        Task<IdentityResult> Create(IdentityUser user, string password);
        Task<IdentityResult> Delete(IdentityUser user);
        Task<IdentityResult> Update(IdentityUser user);
        UserManager<IdentityUser> GetUserManager();
        Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password);
    }
}
