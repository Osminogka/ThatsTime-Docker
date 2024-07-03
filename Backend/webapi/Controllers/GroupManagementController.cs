using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System.Security.Claims;
using webapi.Models;

namespace webapi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/groups")]
    public class GroupManagementController : MyBaseController
    {
        DataContext DataContext;

        public GroupManagementController(DataContext ctx)
        {
            DataContext = ctx;
        }

        [HttpGet("getgroups")]
        public async Task<IActionResult> getGroupsAsync([FromQuery] int page)
        {
            GroupResponse response = new GroupResponse();
            const int pageSize = 5;
            try
            {
                string mainUserName = getUserName();
                response.Groups.AddRange(await DataContext.GroupsCreatorsLists
                    .Where(obj => obj.IsGroupClosed == false && obj.GroupMembers.Where(member => member.RelatedUser.UserName == mainUserName).Count() == 0)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .Select(obj => obj.GroupName)
                    .ToListAsync());
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Got all groups";
            return Ok(response);
        }

        [HttpGet("getcertaingroup")]
        public async Task<IActionResult> getCertainGroupAsync([FromQuery] string groupname)
        {
            GroupResponse response = new GroupResponse();
            try
            {
                string mainUserName = getUserName();
                GroupsCreatorsList? group = await DataContext.GroupsCreatorsLists
                    .SingleOrDefaultAsync(obj => obj.GroupName == groupname && obj.IsGroupClosed == false && obj.GroupMembers.Where(member => member.RelatedUser.UserName == mainUserName).Count() == 0);
                if(group == null)
                {
                    response.Message = "Such groupo doesn't exist";
                    return Ok(response);
                }

                response.Groups.Add(group.GroupName);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Got certain group";
            return Ok(response);
        }

        [HttpGet("getmygroups")]
        public async Task<IActionResult> getMyGroupsAsync()
        {
            GroupResponse response = new GroupResponse();
            try
            {
                response.Groups.AddRange(await DataContext.GroupMemberLists.Where(obj => obj.RelatedUser.UserName == getUserName()).Select(obj => obj.RelatedGroup.GroupName).ToListAsync());
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
            response.Success = true;
            response.Message = "Got all groups";
            return Ok(response);
        } 

        [HttpPost("create")]
        public async Task<IActionResult> createGroupAsync([FromQuery] string groupname)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                UserInfo? mainUser = await DataContext.UserInfo.SingleOrDefaultAsync(obj => obj.UserName == getUserName());
                if (mainUser == null)
                    return Ok(response);

                GroupsCreatorsList? group = await DataContext.GroupsCreatorsLists.SingleOrDefaultAsync(obj => obj.GroupName == groupname);
                if (group != null)
                {
                    response.Message = "Such group already exist";
                    return Ok(response);
                }

                GroupsCreatorsList newGroup = new GroupsCreatorsList()
                {
                    GroupName = groupname,
                    CreatorId = mainUser.Id,
                };

                await DataContext.GroupsCreatorsLists.AddAsync(newGroup);
                await DataContext.SaveChangesAsync();

                GroupMemberList newMember = new GroupMemberList()
                {
                    MemberId = mainUser.Id,
                    Id = newGroup.Id,
                    RoleId = DataContext.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Creator")?.Id ?? 0
                };

                await DataContext.GroupMemberLists.AddAsync(newMember);
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Group created successfully";
            return Ok(response);
        }

        [HttpGet("enter")]
        public async Task<IActionResult> enterGroupAsync([FromQuery] string groupname)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                UserInfo? mainUser = await DataContext.UserInfo.SingleOrDefaultAsync(obj => obj.UserName == getUserName());
                if (mainUser == null)
                    return Ok(response);

                GroupsCreatorsList? group = await DataContext.GroupsCreatorsLists
                    .Include(member => member.GroupMembers.Where(user => user.MemberId == mainUser.Id))
                    .SingleOrDefaultAsync(group => group.GroupName == groupname && group.IsGroupClosed == false);

                if (group == null || group?.GroupMembers.Count > 0)
                    return Ok(response);

                GroupMemberList newMember = new GroupMemberList()
                {
                    Id = group.Id,
                    MemberId = mainUser.Id,
                    RoleId = DataContext.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Member")?.Id ?? 0
                };

                await DataContext.GroupMemberLists.AddAsync(newMember);
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Successfully entered the group";
            return Ok(response);
        }

        [HttpGet("getinvites")]
        public async Task<IActionResult> getGroupInvitesAsync()
        {
            GroupResponse response = new GroupResponse();

            try
            {
                UserInfo? mainUser = await DataContext.UserInfo
                    .Include(obj => obj.GroupInvites).ThenInclude(obj => obj.GroupEntity)
                    .SingleOrDefaultAsync(obj => obj.UserName == getUserName());

                if (mainUser == null)
                    return Ok(response);

                response.Groups = mainUser.GroupInvites.Select(obj => obj.GroupEntity.GroupName).ToList();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Got all invites";
            return Ok(response);
        }

        [HttpGet("sendinvite")]
        public async Task<IActionResult> sendInviteToGroupAsync([FromQuery] string groupName, [FromQuery] string friendName)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                var isInviteExist = await DataContext.GroupInvites.SingleOrDefaultAsync(obj => obj.GroupEntity.GroupName == groupName && obj.UserEntity.UserName == friendName);
                if(isInviteExist != null)
                {
                    response.Message = "Invite already exist";
                    return Ok(response);
                }

                UserInfo? mainUser = await DataContext.UserInfo
                    .Include(user => user.GroupMembers.Where(group => group.RelatedGroup.GroupName == groupName)).ThenInclude(obj => obj.RelatedGroup)
                    .Include(member => member.GroupMembers).ThenInclude(member => member.Role)
                    .SingleOrDefaultAsync(obj => obj.UserName == getUserName());

                if (mainUser == null)
                {
                    response.Message = "User doesn't exist";
                    return Ok(response);
                }

                var groupMember = mainUser.GroupMembers.SingleOrDefault(group => group.RelatedGroup.GroupName == groupName && group.MemberId == mainUser.Id &&
                    (group.Role.RoleName == "Moderator" || group.Role.RoleName == "Creator"));
                if (groupMember == null)
                {
                    response.Message = "You are not permitted to send invites";
                    return Ok(response);
                }

                UserInfo? friend = await DataContext.UserInfo.SingleOrDefaultAsync(obj => obj.UserName == friendName);
                if (friend == null)
                {
                    response.Message = "User doesn't exist";
                    return Ok(response);
                }

                long firstUserId = Math.Max(mainUser.Id, friend.Id);
                long secondUserId = Math.Min(mainUser.Id, friend.Id);

                FriendsList? areFriends = await DataContext.FriendsLists.SingleOrDefaultAsync(obj => obj.FirstUserId == firstUserId && obj.SecondUserId == secondUserId);
                if (areFriends == null)
                {
                    response.Message = "This user isn't your friend";
                    return Ok(response);
                }

                var isUserInGroup = await DataContext.GroupMemberLists.SingleOrDefaultAsync(obj => obj.RelatedGroup.GroupName == groupName && obj.RelatedUser.UserName == friendName);
                if(isUserInGroup != null)
                {
                    response.Message = "User already in group";
                    return Ok(response);
                }

                GroupInvites groupInvite = new GroupInvites()
                {
                    Id = groupMember.Id,
                    TargetUserId = friend.Id
                };


                await DataContext.GroupInvites.AddAsync(groupInvite);
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Group invite request is sent";

            return Ok(response);
        }

        [HttpGet("acceptinvite")]
        public async Task<IActionResult> acceptInviteToGroupAsync(string groupname)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                UserInfo? mainUser = await DataContext.UserInfo
                    .Include(obj => obj.GroupInvites).ThenInclude(obj => obj.GroupEntity)
                    .SingleOrDefaultAsync(obj => obj.UserName == getUserName());
                if (mainUser == null)
                    return Ok(response);

                var groupInvite = mainUser.GroupInvites.SingleOrDefault(invite => invite.GroupEntity.GroupName == groupname && invite.TargetUserId == mainUser.Id);

                if (groupInvite == null)
                {
                    response.Message = "Invite doesn't exist";
                    return Ok(response);
                }

                GroupMemberList newMember = new GroupMemberList()
                {
                    Id = groupInvite.Id,
                    MemberId = mainUser.Id,
                    RoleId = DataContext.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Member")?.Id ?? 0
                };

                DataContext.GroupInvites.Remove(groupInvite);
                await DataContext.GroupMemberLists.AddAsync(newMember);
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Successfully enterd group";
            return Ok(response);
        }

        [HttpGet("declineinvite")]
        public async Task<IActionResult> declineInviteToGroupAsync([FromQuery]string groupname)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                UserInfo? mainUser = await DataContext.UserInfo
                    .Include(obj => obj.GroupInvites).ThenInclude(obj => obj.GroupEntity)
                    .SingleOrDefaultAsync(obj => obj.UserName == getUserName());
                if (mainUser == null)
                    return Ok(response);

                var groupInvite = mainUser.GroupInvites.SingleOrDefault(invite => invite.GroupEntity.GroupName == groupname && invite.TargetUserId == mainUser.Id);

                if (groupInvite == null)
                {
                    response.Message = "Invite doesn't exist";
                    return Ok(response);
                }

                DataContext.GroupInvites.Remove(groupInvite);
                await DataContext.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Successfully decline group invite";
            return Ok(response);
        }

        [HttpGet("remove")]
        public async Task<IActionResult> removeUserFromGroupAsync([FromQuery] string groupName, [FromQuery] string friendName)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                GroupsCreatorsList? group = await DataContext.GroupsCreatorsLists.SingleOrDefaultAsync(obj => obj.GroupName == groupName && obj.Creator.UserName == getUserName());
                if (group == null)
                {
                    response.Message = "You are not creator of the group";
                    return Ok(response);
                }
                GroupMemberList? member = await DataContext.GroupMemberLists.SingleOrDefaultAsync(obj => obj.Id == group.Id && obj.RelatedUser.UserName == friendName);
                if(member == null)
                {
                    response.Message = "This user isn't member of this group";
                    return Ok(response);
                }
                DataContext.GroupMemberLists.Remove(member);
                await DataContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
            response.Success = true;
            response.Message = "Request has succeeded";
            return Ok(response);
        }

        [HttpGet("leave")]
        public async Task<IActionResult> leaveGroupAsync(string groupname)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                string mainUsername = getUserName();

                GroupsCreatorsList? group = await DataContext.GroupsCreatorsLists
                    .Include(obj => obj.GroupMembers)
                    .Include(obj => obj.RecordsForThisGroup)
                    .Include(obj => obj.GroupInvites)
                    .Include(group => group.Creator)
                    .Include(group => group.GroupMembers
                        .Where(member => member.RelatedUser.UserName == mainUsername))
                        .ThenInclude(member => member.RelatedUser)
                    .SingleOrDefaultAsync(obj => obj.GroupName == groupname);
                if (group == null)
                {
                    response.Message = "Such group doesn't exist";
                    return Ok(response);
                }
                GroupMemberList? groupMember = group.GroupMembers.Where(member => member.RelatedUser.UserName == mainUsername).First();
                if (groupMember == null)
                {
                    response.Message = "You are not a member of such group";
                    return Ok(response);
                }
                if(group.Creator.UserName == mainUsername)
                {
                    var firstMember = await DataContext.GroupMemberLists.FirstOrDefaultAsync(obj => obj.Id == group.Id && obj.RelatedUser.UserName != mainUsername);
                    if (firstMember == null)
                    {
                        DataContext.GroupsCreatorsLists.Remove(group);
                        DataContext.GroupMemberLists.RemoveRange(group.GroupMembers);
                        DataContext.Records.RemoveRange(group.RecordsForThisGroup);
                        DataContext.GroupInvites.RemoveRange(group.GroupInvites);
                        DataContext.GroupsCreatorsLists.Remove(group);
                    }
                    else
                    {
                        firstMember.RoleId = DataContext.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Creator")?.Id ?? 0;
                        group.CreatorId = firstMember.MemberId;
                    }
                }

                DataContext.GroupMemberLists.Remove(group.GroupMembers.Where(member => member.RelatedUser.UserName == mainUsername).First());
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Successfuly leaved the group";
            return Ok(response);
        }

        [HttpGet("deletegroup")]
        public async Task<IActionResult> deleteGroupAsync([FromQuery] string groupName)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                GroupsCreatorsList? group = await DataContext.GroupsCreatorsLists
                    .Include(obj => obj.GroupMembers)
                    .Include(obj => obj.RecordsForThisGroup)
                    .Include(obj => obj.GroupInvites)
                    .SingleOrDefaultAsync(obj => obj.GroupName == groupName && obj.Creator.UserName == getUserName());
                if(group == null)
                {
                    response.Message = "Such group doesn't exist";
                    return Ok(response);
                }

                DataContext.GroupMemberLists.RemoveRange(group.GroupMembers);
                DataContext.Records.RemoveRange(group.RecordsForThisGroup);
                DataContext.GroupInvites.RemoveRange(group.GroupInvites);
                DataContext.GroupsCreatorsLists.Remove(group);
                await DataContext.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "Group deleted successfuly";
            return Ok(response);
        }

        [HttpGet("promote")]
        public async Task<IActionResult> promoteUserAsync([FromQuery] string groupName, [FromQuery] string memberName)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                GroupMemberList? member = await DataContext.GroupMemberLists
                    .SingleOrDefaultAsync(obj => obj.RelatedUser.UserName == memberName && obj.RelatedGroup.GroupName == groupName);

                if (member == null)
                {
                    response.Message = "You are not member of this group";
                    return Ok(response);
                }

                member.RoleId = DataContext.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Moderator")?.Id ?? 0;
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "User promoted";
            return Ok(response);
        }

        [HttpGet("demote")]
        public async Task<IActionResult> demoteUserAsync([FromQuery] string groupName, [FromQuery] string memberName)
        {
            GroupResponse response = new GroupResponse();

            try
            {
                GroupMemberList? member = await DataContext.GroupMemberLists
                    .SingleOrDefaultAsync(obj => obj.RelatedUser.UserName == memberName && obj.RelatedGroup.GroupName == groupName);

                if (member == null)
                {
                    response.Message = "You are not member of this group";
                    return Ok(response);
                }

                member.RoleId = DataContext.MemberRoles.SingleOrDefault(obj => obj.RoleName == "Member")?.Id ?? 0;
                await DataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }

            response.Success = true;
            response.Message = "User demoted";
            return Ok(response);
        }
    }

    public class GroupResponse
    {
        public bool Success { get; set; } = false;

        public string Message { get; set; } = "Request has failed";

        public List<string> Groups { get; set; } = new List<string>();
    }

    public class GroupRequest
    {
        public string FriendName { get; set; }

        public string GroupName { get; set; }
    }

}