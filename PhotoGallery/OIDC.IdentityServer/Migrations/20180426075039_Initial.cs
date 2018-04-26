using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OIDC.IdentityServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    Username = table.Column<string>(maxLength: 100, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Users", x => x.SubjectId);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 50, nullable: false),
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    ClaimType = table.Column<string>(maxLength: 250, nullable: false),
                    ClaimValue = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Users_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Users",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logins",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SubjectId = table.Column<string>(maxLength: 50, nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 250, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("UserLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Logins_Users_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Users",
                        principalColumn: "SubjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "SubjectId", "IsActive", "Password", "Username" },
                values: new object[] { "d860efca-22d9-47fd-8249-791ba61b07c7", true, "P@ssw0rd!", "Frank" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "SubjectId", "IsActive", "Password", "Username" },
                values: new object[] { "b7539694-97e7-4dfe-84da-b4256e1ff5c7", true, "P@ssw0rd!", "Claire" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d51211d4-48be-11e8-842f-0ed5f89f718b", "role", "FreeUser", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d512162a-48be-11e8-842f-0ed5f89f718b", "given_name", "Frank", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d51218be-48be-11e8-842f-0ed5f89f718b", "family_name", "Underwood", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d5121e5e-48be-11e8-842f-0ed5f89f718b", "address", "1 Main Road", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d51220c0-48be-11e8-842f-0ed5f89f718b", "subscriptionlevel", "FreeUser", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d51222e6-48be-11e8-842f-0ed5f89f718b", "country", "nl", "d860efca-22d9-47fd-8249-791ba61b07c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d5122516-48be-11e8-842f-0ed5f89f718b", "role", "PayingUser", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d512273c-48be-11e8-842f-0ed5f89f718b", "given_name", "Claire", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d512294e-48be-11e8-842f-0ed5f89f718b", "family_name", "Underwood", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d5122f52-48be-11e8-842f-0ed5f89f718b", "address", "1 Big Street", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d5123222-48be-11e8-842f-0ed5f89f718b", "subscriptionlevel", "PayingUser", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.InsertData(
                table: "Claims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "SubjectId" },
                values: new object[] { "d512345c-48be-11e8-842f-0ed5f89f718b", "country", "be", "b7539694-97e7-4dfe-84da-b4256e1ff5c7" });

            migrationBuilder.CreateIndex(
                name: "IX_Claims_SubjectId",
                table: "Claims",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Logins_SubjectId",
                table: "Logins",
                column: "SubjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Logins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
