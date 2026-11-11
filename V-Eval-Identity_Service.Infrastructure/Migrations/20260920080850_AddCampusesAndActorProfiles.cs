using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace V_Eval_Identity_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCampusesAndActorProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "academic_directors",
                schema: "profile",
                columns: table => new
                {
                    director_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_directors", x => x.director_id);
                    table.ForeignKey(
                        name: "FK_academic_directors_users_director_id",
                        column: x => x.director_id,
                        principalSchema: "iam",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "administrators",
                schema: "profile",
                columns: table => new
                {
                    admin_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_administrators", x => x.admin_id);
                    table.ForeignKey(
                        name: "FK_administrators_users_admin_id",
                        column: x => x.admin_id,
                        principalSchema: "iam",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campuses",
                schema: "iam",
                columns: table => new
                {
                    campus_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    address = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campuses", x => x.campus_id);
                });

            migrationBuilder.CreateTable(
                name: "parents",
                schema: "profile",
                columns: table => new
                {
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_work = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parents", x => x.parent_id);
                    table.ForeignKey(
                        name: "FK_parents_users_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "iam",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "academic_managers",
                schema: "profile",
                columns: table => new
                {
                    manager_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campus_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_academic_managers", x => x.manager_id);
                    table.ForeignKey(
                        name: "FK_academic_managers_campuses_campus_id",
                        column: x => x.campus_id,
                        principalSchema: "iam",
                        principalTable: "campuses",
                        principalColumn: "campus_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_academic_managers_users_manager_id",
                        column: x => x.manager_id,
                        principalSchema: "iam",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "teachers",
                schema: "profile",
                columns: table => new
                {
                    teacher_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campus_id = table.Column<Guid>(type: "uuid", nullable: false),
                    specialty = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    bio = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers", x => x.teacher_id);
                    table.ForeignKey(
                        name: "FK_teachers_campuses_campus_id",
                        column: x => x.campus_id,
                        principalSchema: "iam",
                        principalTable: "campuses",
                        principalColumn: "campus_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_teachers_users_teacher_id",
                        column: x => x.teacher_id,
                        principalSchema: "iam",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "parent_student_relations",
                schema: "profile",
                columns: table => new
                {
                    relation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parent_student_relations", x => x.relation_id);
                    table.ForeignKey(
                        name: "FK_parent_student_relations_parents_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "profile",
                        principalTable: "parents",
                        principalColumn: "parent_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_parent_student_relations_students_student_id",
                        column: x => x.student_id,
                        principalSchema: "profile",
                        principalTable: "students",
                        principalColumn: "student_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "iam",
                table: "campuses",
                columns: new[] { "campus_id", "address", "code", "created_at", "is_active", "name", "phone" },
                values: new object[,]
                {
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "Khu phố 6, Phường Linh Trung, TP. Thủ Đức, TP.HCM", "CS_THU_DUC", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Cơ sở 1 - Khu đô thị ĐHQG-HCM, TP. Thủ Đức", "02837242160" },
                    { new Guid("4ba85f64-5717-4562-b3fc-2c963f66afa7"), "268 Lý Thường Kiệt, Phường 14, Quận 10, TP.HCM", "CS_QUAN_10", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "Cơ sở 2 - Quận 10, TP.HCM", "02838651670" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_students_campus_id",
                schema: "profile",
                table: "students",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_academic_managers_campus_id",
                schema: "profile",
                table: "academic_managers",
                column: "campus_id");

            migrationBuilder.CreateIndex(
                name: "IX_campuses_code",
                schema: "iam",
                table: "campuses",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_parent_student_relations_parent_id",
                schema: "profile",
                table: "parent_student_relations",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_parent_student_relations_student_id",
                schema: "profile",
                table: "parent_student_relations",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_teachers_campus_id",
                schema: "profile",
                table: "teachers",
                column: "campus_id");

            migrationBuilder.AddForeignKey(
                name: "FK_students_campuses_campus_id",
                schema: "profile",
                table: "students",
                column: "campus_id",
                principalSchema: "iam",
                principalTable: "campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_students_campuses_campus_id",
                schema: "profile",
                table: "students");

            migrationBuilder.DropTable(
                name: "academic_directors",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "academic_managers",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "administrators",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "parent_student_relations",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "teachers",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "parents",
                schema: "profile");

            migrationBuilder.DropTable(
                name: "campuses",
                schema: "iam");

            migrationBuilder.DropIndex(
                name: "IX_students_campus_id",
                schema: "profile",
                table: "students");
        }
    }
}
