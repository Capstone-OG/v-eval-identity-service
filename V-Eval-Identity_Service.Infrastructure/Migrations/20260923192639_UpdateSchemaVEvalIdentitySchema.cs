using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace V_Eval_Identity_Service.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchemaVEvalIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_academic_directors_users_director_id",
                schema: "profile",
                table: "academic_directors");

            migrationBuilder.DropForeignKey(
                name: "FK_academic_managers_campuses_campus_id",
                schema: "profile",
                table: "academic_managers");

            migrationBuilder.DropForeignKey(
                name: "FK_academic_managers_users_manager_id",
                schema: "profile",
                table: "academic_managers");

            migrationBuilder.DropForeignKey(
                name: "FK_administrators_users_admin_id",
                schema: "profile",
                table: "administrators");

            migrationBuilder.DropForeignKey(
                name: "FK_parent_student_relations_parents_parent_id",
                schema: "profile",
                table: "parent_student_relations");

            migrationBuilder.DropForeignKey(
                name: "FK_parent_student_relations_students_student_id",
                schema: "profile",
                table: "parent_student_relations");

            migrationBuilder.DropForeignKey(
                name: "FK_parents_users_parent_id",
                schema: "profile",
                table: "parents");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_user_id",
                schema: "iam",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_students_campuses_campus_id",
                schema: "profile",
                table: "students");

            migrationBuilder.DropForeignKey(
                name: "FK_students_users_student_id",
                schema: "profile",
                table: "students");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_campuses_campus_id",
                schema: "profile",
                table: "teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_teachers_users_teacher_id",
                schema: "profile",
                table: "teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                schema: "iam",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                schema: "iam",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                schema: "iam",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_teachers",
                schema: "profile",
                table: "teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_students",
                schema: "profile",
                table: "students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                schema: "iam",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_parents",
                schema: "profile",
                table: "parents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_campuses",
                schema: "iam",
                table: "campuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_administrators",
                schema: "profile",
                table: "administrators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                schema: "iam",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                schema: "iam",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_parent_student_relations",
                schema: "profile",
                table: "parent_student_relations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_otp_verifications",
                schema: "iam",
                table: "otp_verifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_academic_managers",
                schema: "profile",
                table: "academic_managers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_academic_directors",
                schema: "profile",
                table: "academic_directors");

            migrationBuilder.EnsureSchema(
                name: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "users",
                schema: "iam",
                newName: "Users",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "teachers",
                schema: "profile",
                newName: "Teachers",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "students",
                schema: "profile",
                newName: "Students",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "roles",
                schema: "iam",
                newName: "Roles",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "parents",
                schema: "profile",
                newName: "Parents",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "campuses",
                schema: "iam",
                newName: "Campuses",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "administrators",
                schema: "profile",
                newName: "Administrators",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "user_roles",
                schema: "iam",
                newName: "UserRoles",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "refresh_tokens",
                schema: "iam",
                newName: "RefreshTokens",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "parent_student_relations",
                schema: "profile",
                newName: "ParentStudentRelations",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "otp_verifications",
                schema: "iam",
                newName: "OtpVerifications",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "academic_managers",
                schema: "profile",
                newName: "AcademicManagers",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameTable(
                name: "academic_directors",
                schema: "profile",
                newName: "AcademicDirectors",
                newSchema: "v_eval_identity");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                schema: "v_eval_identity",
                table: "Users",
                newName: "IX_Users_email");

            migrationBuilder.RenameIndex(
                name: "IX_teachers_campus_id",
                schema: "v_eval_identity",
                table: "Teachers",
                newName: "IX_Teachers_campus_id");

            migrationBuilder.RenameIndex(
                name: "IX_students_campus_id",
                schema: "v_eval_identity",
                table: "Students",
                newName: "IX_Students_campus_id");

            migrationBuilder.RenameIndex(
                name: "IX_roles_role_name",
                schema: "v_eval_identity",
                table: "Roles",
                newName: "IX_Roles_role_name");

            migrationBuilder.RenameIndex(
                name: "IX_campuses_code",
                schema: "v_eval_identity",
                table: "Campuses",
                newName: "IX_Campuses_code");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_role_id",
                schema: "v_eval_identity",
                table: "UserRoles",
                newName: "IX_UserRoles_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_user_id",
                schema: "v_eval_identity",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_parent_student_relations_student_id",
                schema: "v_eval_identity",
                table: "ParentStudentRelations",
                newName: "IX_ParentStudentRelations_student_id");

            migrationBuilder.RenameIndex(
                name: "IX_parent_student_relations_parent_id",
                schema: "v_eval_identity",
                table: "ParentStudentRelations",
                newName: "IX_ParentStudentRelations_parent_id");

            migrationBuilder.RenameIndex(
                name: "IX_academic_managers_campus_id",
                schema: "v_eval_identity",
                table: "AcademicManagers",
                newName: "IX_AcademicManagers_campus_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                schema: "v_eval_identity",
                table: "Users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Teachers",
                schema: "v_eval_identity",
                table: "Teachers",
                column: "teacher_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                schema: "v_eval_identity",
                table: "Students",
                column: "student_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                schema: "v_eval_identity",
                table: "Roles",
                column: "role_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parents",
                schema: "v_eval_identity",
                table: "Parents",
                column: "parent_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Campuses",
                schema: "v_eval_identity",
                table: "Campuses",
                column: "campus_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Administrators",
                schema: "v_eval_identity",
                table: "Administrators",
                column: "admin_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                schema: "v_eval_identity",
                table: "UserRoles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "v_eval_identity",
                table: "RefreshTokens",
                column: "refresh_token_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ParentStudentRelations",
                schema: "v_eval_identity",
                table: "ParentStudentRelations",
                column: "relation_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerifications",
                schema: "v_eval_identity",
                table: "OtpVerifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicManagers",
                schema: "v_eval_identity",
                table: "AcademicManagers",
                column: "manager_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AcademicDirectors",
                schema: "v_eval_identity",
                table: "AcademicDirectors",
                column: "director_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicDirectors_Users_director_id",
                schema: "v_eval_identity",
                table: "AcademicDirectors",
                column: "director_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicManagers_Campuses_campus_id",
                schema: "v_eval_identity",
                table: "AcademicManagers",
                column: "campus_id",
                principalSchema: "v_eval_identity",
                principalTable: "Campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicManagers_Users_manager_id",
                schema: "v_eval_identity",
                table: "AcademicManagers",
                column: "manager_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Administrators_Users_admin_id",
                schema: "v_eval_identity",
                table: "Administrators",
                column: "admin_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_Users_parent_id",
                schema: "v_eval_identity",
                table: "Parents",
                column: "parent_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudentRelations_Parents_parent_id",
                schema: "v_eval_identity",
                table: "ParentStudentRelations",
                column: "parent_id",
                principalSchema: "v_eval_identity",
                principalTable: "Parents",
                principalColumn: "parent_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParentStudentRelations_Students_student_id",
                schema: "v_eval_identity",
                table: "ParentStudentRelations",
                column: "student_id",
                principalSchema: "v_eval_identity",
                principalTable: "Students",
                principalColumn: "student_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_user_id",
                schema: "v_eval_identity",
                table: "RefreshTokens",
                column: "user_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Campuses_campus_id",
                schema: "v_eval_identity",
                table: "Students",
                column: "campus_id",
                principalSchema: "v_eval_identity",
                principalTable: "Campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Users_student_id",
                schema: "v_eval_identity",
                table: "Students",
                column: "student_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Campuses_campus_id",
                schema: "v_eval_identity",
                table: "Teachers",
                column: "campus_id",
                principalSchema: "v_eval_identity",
                principalTable: "Campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Users_teacher_id",
                schema: "v_eval_identity",
                table: "Teachers",
                column: "teacher_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_role_id",
                schema: "v_eval_identity",
                table: "UserRoles",
                column: "role_id",
                principalSchema: "v_eval_identity",
                principalTable: "Roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_user_id",
                schema: "v_eval_identity",
                table: "UserRoles",
                column: "user_id",
                principalSchema: "v_eval_identity",
                principalTable: "Users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicDirectors_Users_director_id",
                schema: "v_eval_identity",
                table: "AcademicDirectors");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicManagers_Campuses_campus_id",
                schema: "v_eval_identity",
                table: "AcademicManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_AcademicManagers_Users_manager_id",
                schema: "v_eval_identity",
                table: "AcademicManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_Administrators_Users_admin_id",
                schema: "v_eval_identity",
                table: "Administrators");

            migrationBuilder.DropForeignKey(
                name: "FK_Parents_Users_parent_id",
                schema: "v_eval_identity",
                table: "Parents");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudentRelations_Parents_parent_id",
                schema: "v_eval_identity",
                table: "ParentStudentRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_ParentStudentRelations_Students_student_id",
                schema: "v_eval_identity",
                table: "ParentStudentRelations");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_user_id",
                schema: "v_eval_identity",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Campuses_campus_id",
                schema: "v_eval_identity",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Users_student_id",
                schema: "v_eval_identity",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Campuses_campus_id",
                schema: "v_eval_identity",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Users_teacher_id",
                schema: "v_eval_identity",
                table: "Teachers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_role_id",
                schema: "v_eval_identity",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_user_id",
                schema: "v_eval_identity",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                schema: "v_eval_identity",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Teachers",
                schema: "v_eval_identity",
                table: "Teachers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                schema: "v_eval_identity",
                table: "Students");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                schema: "v_eval_identity",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parents",
                schema: "v_eval_identity",
                table: "Parents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Campuses",
                schema: "v_eval_identity",
                table: "Campuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Administrators",
                schema: "v_eval_identity",
                table: "Administrators");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                schema: "v_eval_identity",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                schema: "v_eval_identity",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ParentStudentRelations",
                schema: "v_eval_identity",
                table: "ParentStudentRelations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerifications",
                schema: "v_eval_identity",
                table: "OtpVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicManagers",
                schema: "v_eval_identity",
                table: "AcademicManagers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AcademicDirectors",
                schema: "v_eval_identity",
                table: "AcademicDirectors");

            migrationBuilder.EnsureSchema(
                name: "profile");

            migrationBuilder.EnsureSchema(
                name: "iam");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "v_eval_identity",
                newName: "users",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "Teachers",
                schema: "v_eval_identity",
                newName: "teachers",
                newSchema: "profile");

            migrationBuilder.RenameTable(
                name: "Students",
                schema: "v_eval_identity",
                newName: "students",
                newSchema: "profile");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "v_eval_identity",
                newName: "roles",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "Parents",
                schema: "v_eval_identity",
                newName: "parents",
                newSchema: "profile");

            migrationBuilder.RenameTable(
                name: "Campuses",
                schema: "v_eval_identity",
                newName: "campuses",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "Administrators",
                schema: "v_eval_identity",
                newName: "administrators",
                newSchema: "profile");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                schema: "v_eval_identity",
                newName: "user_roles",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                schema: "v_eval_identity",
                newName: "refresh_tokens",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "ParentStudentRelations",
                schema: "v_eval_identity",
                newName: "parent_student_relations",
                newSchema: "profile");

            migrationBuilder.RenameTable(
                name: "OtpVerifications",
                schema: "v_eval_identity",
                newName: "otp_verifications",
                newSchema: "iam");

            migrationBuilder.RenameTable(
                name: "AcademicManagers",
                schema: "v_eval_identity",
                newName: "academic_managers",
                newSchema: "profile");

            migrationBuilder.RenameTable(
                name: "AcademicDirectors",
                schema: "v_eval_identity",
                newName: "academic_directors",
                newSchema: "profile");

            migrationBuilder.RenameIndex(
                name: "IX_Users_email",
                schema: "iam",
                table: "users",
                newName: "IX_users_email");

            migrationBuilder.RenameIndex(
                name: "IX_Teachers_campus_id",
                schema: "profile",
                table: "teachers",
                newName: "IX_teachers_campus_id");

            migrationBuilder.RenameIndex(
                name: "IX_Students_campus_id",
                schema: "profile",
                table: "students",
                newName: "IX_students_campus_id");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_role_name",
                schema: "iam",
                table: "roles",
                newName: "IX_roles_role_name");

            migrationBuilder.RenameIndex(
                name: "IX_Campuses_code",
                schema: "iam",
                table: "campuses",
                newName: "IX_campuses_code");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_role_id",
                schema: "iam",
                table: "user_roles",
                newName: "IX_user_roles_role_id");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_user_id",
                schema: "iam",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_ParentStudentRelations_student_id",
                schema: "profile",
                table: "parent_student_relations",
                newName: "IX_parent_student_relations_student_id");

            migrationBuilder.RenameIndex(
                name: "IX_ParentStudentRelations_parent_id",
                schema: "profile",
                table: "parent_student_relations",
                newName: "IX_parent_student_relations_parent_id");

            migrationBuilder.RenameIndex(
                name: "IX_AcademicManagers_campus_id",
                schema: "profile",
                table: "academic_managers",
                newName: "IX_academic_managers_campus_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                schema: "iam",
                table: "users",
                column: "user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_teachers",
                schema: "profile",
                table: "teachers",
                column: "teacher_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_students",
                schema: "profile",
                table: "students",
                column: "student_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                schema: "iam",
                table: "roles",
                column: "role_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_parents",
                schema: "profile",
                table: "parents",
                column: "parent_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_campuses",
                schema: "iam",
                table: "campuses",
                column: "campus_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_administrators",
                schema: "profile",
                table: "administrators",
                column: "admin_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                schema: "iam",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                schema: "iam",
                table: "refresh_tokens",
                column: "refresh_token_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_parent_student_relations",
                schema: "profile",
                table: "parent_student_relations",
                column: "relation_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_otp_verifications",
                schema: "iam",
                table: "otp_verifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_academic_managers",
                schema: "profile",
                table: "academic_managers",
                column: "manager_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_academic_directors",
                schema: "profile",
                table: "academic_directors",
                column: "director_id");

            migrationBuilder.AddForeignKey(
                name: "FK_academic_directors_users_director_id",
                schema: "profile",
                table: "academic_directors",
                column: "director_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_academic_managers_campuses_campus_id",
                schema: "profile",
                table: "academic_managers",
                column: "campus_id",
                principalSchema: "iam",
                principalTable: "campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_academic_managers_users_manager_id",
                schema: "profile",
                table: "academic_managers",
                column: "manager_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_administrators_users_admin_id",
                schema: "profile",
                table: "administrators",
                column: "admin_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_parent_student_relations_parents_parent_id",
                schema: "profile",
                table: "parent_student_relations",
                column: "parent_id",
                principalSchema: "profile",
                principalTable: "parents",
                principalColumn: "parent_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_parent_student_relations_students_student_id",
                schema: "profile",
                table: "parent_student_relations",
                column: "student_id",
                principalSchema: "profile",
                principalTable: "students",
                principalColumn: "student_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_parents_users_parent_id",
                schema: "profile",
                table: "parents",
                column: "parent_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_user_id",
                schema: "iam",
                table: "refresh_tokens",
                column: "user_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_students_campuses_campus_id",
                schema: "profile",
                table: "students",
                column: "campus_id",
                principalSchema: "iam",
                principalTable: "campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_students_users_student_id",
                schema: "profile",
                table: "students",
                column: "student_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_campuses_campus_id",
                schema: "profile",
                table: "teachers",
                column: "campus_id",
                principalSchema: "iam",
                principalTable: "campuses",
                principalColumn: "campus_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_teachers_users_teacher_id",
                schema: "profile",
                table: "teachers",
                column: "teacher_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                schema: "iam",
                table: "user_roles",
                column: "role_id",
                principalSchema: "iam",
                principalTable: "roles",
                principalColumn: "role_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                schema: "iam",
                table: "user_roles",
                column: "user_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
