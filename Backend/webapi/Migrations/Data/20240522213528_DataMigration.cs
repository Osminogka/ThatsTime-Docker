using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webapi.Migrations.Data
{
    /// <inheritdoc />
    public partial class DataMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FriendInvites",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderUserId = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendInvites_UserInfo_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendInvites_UserInfo_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FriendsLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstUserId = table.Column<long>(type: "bigint", nullable: false),
                    SecondUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendsLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FriendsLists_UserInfo_FirstUserId",
                        column: x => x.FirstUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendsLists_UserInfo_SecondUserId",
                        column: x => x.SecondUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupsCreatorsLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    IsGroupClosed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupsCreatorsLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupsCreatorsLists_UserInfo_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupInvites",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    TargetUserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupInvites_GroupsCreatorsLists_Id",
                        column: x => x.Id,
                        principalTable: "GroupsCreatorsLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupInvites_UserInfo_TargetUserId",
                        column: x => x.TargetUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMemberLists",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    MemberId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMemberLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMemberLists_GroupsCreatorsLists_Id",
                        column: x => x.Id,
                        principalTable: "GroupsCreatorsLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMemberLists_MemberRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "MemberRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupMemberLists_UserInfo_MemberId",
                        column: x => x.MemberId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRecordForGroup = table.Column<bool>(type: "bit", nullable: false),
                    RelatedUserId = table.Column<long>(type: "bigint", nullable: true),
                    RelatedGroupId = table.Column<long>(type: "bigint", nullable: true),
                    CreatorId = table.Column<long>(type: "bigint", nullable: false),
                    IsRecordForYourSelf = table.Column<bool>(type: "bit", nullable: false),
                    Importance = table.Column<int>(type: "int", nullable: false),
                    RecordName = table.Column<string>(type: "nvarchar(50)", maxLength: 1, nullable: false),
                    RecordContent = table.Column<string>(type: "nvarchar(500)", maxLength: 1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Records_GroupsCreatorsLists_RelatedGroupId",
                        column: x => x.RelatedGroupId,
                        principalTable: "GroupsCreatorsLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Records_UserInfo_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Records_UserInfo_RelatedUserId",
                        column: x => x.RelatedUserId,
                        principalTable: "UserInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendInvites_SenderUserId",
                table: "FriendInvites",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendInvites_TargetUserId",
                table: "FriendInvites",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendsLists_FirstUserId",
                table: "FriendsLists",
                column: "FirstUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendsLists_SecondUserId",
                table: "FriendsLists",
                column: "SecondUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInvites_TargetUserId",
                table: "GroupInvites",
                column: "TargetUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberLists_MemberId",
                table: "GroupMemberLists",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMemberLists_RoleId",
                table: "GroupMemberLists",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupsCreatorsLists_CreatorId",
                table: "GroupsCreatorsLists",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_CreatorId",
                table: "Records",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_RelatedGroupId",
                table: "Records",
                column: "RelatedGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Records_RelatedUserId",
                table: "Records",
                column: "RelatedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendInvites");

            migrationBuilder.DropTable(
                name: "FriendsLists");

            migrationBuilder.DropTable(
                name: "GroupInvites");

            migrationBuilder.DropTable(
                name: "GroupMemberLists");

            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "MemberRoles");

            migrationBuilder.DropTable(
                name: "GroupsCreatorsLists");

            migrationBuilder.DropTable(
                name: "UserInfo");
        }
    }
}
