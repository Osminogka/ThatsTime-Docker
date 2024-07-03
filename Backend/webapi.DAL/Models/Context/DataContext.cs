using Microsoft.EntityFrameworkCore;

namespace webapi.Models
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> opts): base(opts) { }

        public DbSet<Record> Records => Set<Record>();
        public DbSet<GroupsCreatorsList> GroupsCreatorsLists => Set<GroupsCreatorsList>();
        public DbSet<GroupMemberList> GroupMemberLists => Set<GroupMemberList>();
        public DbSet<GroupInvites> GroupInvites => Set<GroupInvites>();
        public DbSet<FriendsList> FriendsLists => Set<FriendsList>();
        public DbSet<FriendInvites> FriendInvites => Set<FriendInvites>();
        public DbSet<UserInfo> UserInfo => Set<UserInfo>();
        public DbSet<MemberRole> MemberRoles => Set<MemberRole>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberRole>().HasData(
                new MemberRole
                {
                    Id = 1,
                    RoleName = "Undefined"
                },
                new MemberRole
                {
                    Id = 2,
                    RoleName = "Creator"
                },
                new MemberRole
                {
                    Id = 3,
                    RoleName = "Moderator"
                },
                new MemberRole
                {
                    Id = 4,
                    RoleName = "Member"
                }
            );

            //Set value types
            modelBuilder.Entity<Record>()
                .Property(e => e.RecordContent).HasColumnType("nvarchar(500)");

            modelBuilder.Entity<Record>()
                .Property(e => e.RecordName).HasColumnType("nvarchar(50)");

            //Group Main table groupId & Group invites groupId set up
            modelBuilder.Entity<GroupsCreatorsList>()
                .HasMany(e => e.GroupInvites)
                .WithOne(e => e.GroupEntity)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            //Main Group table groupId & Group member groupId set up
            modelBuilder.Entity<GroupsCreatorsList>()
                .HasMany(e => e.GroupMembers)
                .WithOne(e => e.RelatedGroup)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            //UserInfo userId & Group invites userId set up
            modelBuilder.Entity<UserInfo>()
                .HasMany(e => e.GroupInvites)
                .WithOne(e => e.UserEntity)
                .HasForeignKey(e => e.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //UserInfo userId & Main Group table creatorId set up
            modelBuilder.Entity<UserInfo>()
                .HasMany(e => e.CreatorOfGroups)
                .WithOne(e => e.Creator)
                .HasForeignKey(e => e.CreatorId)
                .IsRequired();

            //UserInfo userid & Group member userId set up
            modelBuilder.Entity<UserInfo>()
                .HasMany(e => e.GroupMembers)
                .WithOne(e => e.RelatedUser)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupMemberList>()
                .HasOne(e => e.Role)
                .WithMany(e => e.Members)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            //UserInfo userId & Friend Invites sender userId
            modelBuilder.Entity<FriendInvites>()
                .HasOne(e => e.SenderUserInfo)
                .WithMany(e => e.SenderUsers)
                .HasForeignKey(e => e.SenderUserId)
                .IsRequired();

            //UserInfo userId & Friend Invites targer userId
            modelBuilder.Entity<FriendInvites>()
                .HasOne(e => e.TargetUserInfo)
                .WithMany(e => e.TargetUsers)
                .HasForeignKey(e => e.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //UserInfo userId & Friends List First userId
            modelBuilder.Entity<FriendsList>()
                .HasOne(e => e.FirstUserInfo)
                .WithMany(e => e.FirstFromFriendList)
                .HasForeignKey(e => e.FirstUserId)
                .IsRequired();

            //UserInfo userId & Friend List Secord userId
            modelBuilder.Entity<FriendsList>()
                .HasOne(e => e.SecondUserInfo)
                .WithMany(e => e.SecondFromFriendList)
                .HasForeignKey(e => e.SecondUserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Creator
            modelBuilder.Entity<Record>()
                .HasOne(e => e.CreatorUser)
                .WithMany(e => e.RecordsCreators)
                .HasForeignKey(e => e.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);

            //GroupCreatorList Id & Record related group
            modelBuilder.Entity<Record>()
                .HasOne(e => e.RelatedGroup)
                .WithMany(e => e.RecordsForThisGroup)
                .HasForeignKey(e => e.RelatedGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            //UserInfo userId & Record related user
            modelBuilder.Entity<Record>()
                .HasOne(e => e.RelatedUser)
                .WithMany(e => e.RecordsForThisUser)
                .HasForeignKey(e => e.RelatedUserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
