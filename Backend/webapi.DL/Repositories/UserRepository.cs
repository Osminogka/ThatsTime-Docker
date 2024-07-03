using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace webapi.DL.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UsersRepository(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IQueryable<IdentityUser> Get() => _userManager.Users;

        public async Task<IdentityUser> GetByUsername(string username)
        {
            return await _userManager.Users.SingleOrDefaultAsync(obj => obj.UserName == username);
        }

        public Task<IdentityResult> Create(IdentityUser user, string password)
        {
            return _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> Delete(IdentityUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IdentityResult> Update(IdentityUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public UserManager<IdentityUser> GetUserManager()
        {
            return _userManager;
        }

        public async Task<SignInResult> CheckPasswordSignInAsync(IdentityUser user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }
    }
}
