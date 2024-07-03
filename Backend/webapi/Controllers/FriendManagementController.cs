using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webapi.DL.Infrastructure;
using webapi.DL.Repositories;
using webapi.Models;

namespace webapi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/friends")]
    public class FriendManagementController : MyBaseController
    {
        private readonly IBaseRepository<UserInfo> _userInfoRepository;
        private readonly IBaseRepository<FriendsList> _friendListRepository;
        private readonly IBaseRepository<FriendInvites> _friendInvitesRepository;
        private readonly IGetUsername _getUsername;

        public FriendManagementController(IBaseRepository<UserInfo> userInfo, IBaseRepository<FriendsList> friendList, IBaseRepository<FriendInvites> friendInvites, IGetUsername getUsername)
        {
            _userInfoRepository = userInfo;
            _friendListRepository = friendList;
            _friendInvitesRepository = friendInvites;
            _getUsername = getUsername;
        }

        [HttpGet("getusers")]
        public async Task<IActionResult> getUsersAsync([FromQuery] int page)
        {
            FriendResponse response = new FriendResponse();
            const int pageSize = 5;
            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                response.FriendList.AddRange(await _userInfoRepository
                    .Where(obj => (obj.UserName != mainUsername) &&
                        obj.FirstFromFriendList.Where(friend => friend.FirstUserInfo.UserName == mainUsername || friend.SecondUserInfo.UserName == mainUsername).Count() == 0 &&
                        obj.SecondFromFriendList.Where(friend => friend.FirstUserInfo.UserName == mainUsername || friend.SecondUserInfo.UserName == mainUsername).Count() == 0)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(obj => obj.UserName)
                    .ToListAsync());
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";
            return Ok(response);
        }

        [HttpGet("getcertainuser")]
        public async Task<IActionResult> getCertainUserAsync([FromQuery] string username)
        {
            FriendResponse response = new FriendResponse();
            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext); ;
                UserInfo? certainUser = await _userInfoRepository
                    .SingleOrDefaultAsync(obj => (obj.UserName != mainUsername) && (obj.UserName == username) &&
                        obj.FirstFromFriendList.Where(friend => friend.FirstUserInfo.UserName == mainUsername || friend.SecondUserInfo.UserName == mainUsername).Count() == 0 &&
                        obj.SecondFromFriendList.Where(friend => friend.FirstUserInfo.UserName == mainUsername || friend.SecondUserInfo.UserName == mainUsername).Count() == 0);

                if(certainUser == null)
                {
                    response.Message = "Such user doesn't exist";
                    return Ok(response);
                }

                response.FriendList.Add(certainUser.UserName);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Got certain user";
            return Ok(response);
        }

        [HttpGet("getfriends")]
        public async Task<IActionResult> getFriendListAsync()
        {
            FriendResponse response = new FriendResponse();

            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                List<string> friendList = await _friendListRepository
                    .Where(obj => obj.FirstUserInfo.UserName == mainUsername || obj.SecondUserInfo.UserName == mainUsername)
                    .Select(obj => obj.FirstUserInfo.UserName == mainUsername ? obj.SecondUserInfo.UserName : obj.FirstUserInfo.UserName).ToListAsync();

                response.FriendList = friendList;
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";

            return Ok(response);
        }

        [HttpGet("sendinvite")]
        public async Task<IActionResult> SendFrienInviteToUserAsync([FromQuery]string FriendName)
        {
            FriendResponse response = new FriendResponse();

            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                if (await areFriends(FriendName))
                    return Ok(response);

                FriendInvites? friendInvite = await _friendInvitesRepository
                    .SingleOrDefaultAsync(obj => obj.SenderUserInfo.UserName == mainUsername && obj.TargetUserInfo.UserName == FriendName);

                if (friendInvite != null)
                {
                    response.Success = true;
                    response.Message = "Such invite already exist";
                    return Ok(response);
                }

                UserInfo? mainUser = await _userInfoRepository.SingleOrDefaultAsync(obj => obj.UserName == mainUsername);
                UserInfo? friendUser = await _userInfoRepository.SingleOrDefaultAsync(obj => obj.UserName == FriendName);

                FriendInvites invite = new FriendInvites()
                {
                    SenderUserId = mainUser.Id,
                    TargetUserId = friendUser.Id
                };

                await _friendInvitesRepository.AddAsync(invite);
                await _friendInvitesRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";

            return Ok(response);
        }

        [HttpGet("deletefriend")]
        public async Task<IActionResult> DeleteFriendAsync([FromQuery] string FriendName)
        {
            FriendResponse response = new FriendResponse();
            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                FriendsList? friendsList = await _friendListRepository.SingleOrDefaultAsync(obj => (obj.FirstUserInfo.UserName == mainUsername && obj.SecondUserInfo.UserName == FriendName) ||
                    obj.FirstUserInfo.UserName == FriendName && obj.SecondUserInfo.UserName == mainUsername);
                if (friendsList == null)
                    return Ok(response);

                _friendListRepository.Delete(friendsList);
                await _friendListRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";

            return Ok(response);
        }

        [HttpGet("getinvites")]
        public async Task<IActionResult> getInvitesAsync()
        {
            FriendResponse response = new FriendResponse();

            try
            {
                var invites = await _friendInvitesRepository.Where(obj => obj.TargetUserInfo.UserName == getUserName()).Select(obj => obj.SenderUserInfo.UserName).ToListAsync();

                if (invites != null)
                    response.FriendList = invites;
                else
                    response.FriendList = new List<string>();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";
            return Ok(response);
        }

        [HttpGet("acceptinvite")]
        public async Task<IActionResult> AcceptFriendInvite([FromQuery] string FriendName)
        {
            FriendResponse response = new FriendResponse();
            List<FriendInvites> invites = new List<FriendInvites>();
            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                var toMainUserInvite = await _friendInvitesRepository
                    .SingleOrDefaultAsync(obj => obj.TargetUserInfo.UserName == mainUsername && obj.SenderUserInfo.UserName == FriendName);

                if (toMainUserInvite == null)
                    return Ok(response);

                var fromMainUserInvite = await _friendInvitesRepository
                    .SingleOrDefaultAsync(obj => obj.SenderUserInfo.UserName == mainUsername && obj.TargetUserInfo.UserName == FriendName);

                if (fromMainUserInvite != null)
                    invites.Add(fromMainUserInvite);

                invites.Add(toMainUserInvite);

                long firstUserId = Math.Max(toMainUserInvite.SenderUserId, toMainUserInvite.TargetUserId);
                long secondUserId = Math.Min(toMainUserInvite.SenderUserId, toMainUserInvite.TargetUserId);

                FriendsList friendsList = new FriendsList()
                {
                    FirstUserId = firstUserId,
                    SecondUserId = secondUserId
                };

                _friendInvitesRepository.DeleteRange(invites);
                await _friendListRepository.AddAsync(friendsList);
                await _friendListRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";

            return Ok(response);
        }

        [HttpGet("declineinvite")]
        public async Task<IActionResult> DeclineFriendInvite([FromQuery] string FriendName)
        {
            FriendResponse response = new FriendResponse();
            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                FriendInvites? friendInvite = await _friendInvitesRepository.SingleOrDefaultAsync(obj => obj.TargetUserInfo.UserName == mainUsername && obj.SenderUserInfo.UserName == FriendName);
                if (friendInvite == null)
                    return Ok(response);

                _friendInvitesRepository.Delete(friendInvite);
                await _friendInvitesRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Request has succeeded";

            return Ok(response);
        }

        private async Task<bool> areFriends(string friendName)
        {
            try
            {
                string mainUsername = _getUsername.getUserName(HttpContext);

                FriendsList? areFriends = await _friendListRepository.SingleOrDefaultAsync(obj => (obj.FirstUserInfo.UserName == mainUsername && obj.SecondUserInfo.UserName == friendName) ||
                    (obj.FirstUserInfo.UserName == friendName && obj.SecondUserInfo.UserName == mainUsername));
                if (areFriends == null)
                    return false;

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }


    public class FriendResponse
    {
        public bool Success { get; set; } = true;

        public string Message { get; set; } = "Request has failed";

        public List<string> FriendList { get; set; } = new List<string>();
    }
}
