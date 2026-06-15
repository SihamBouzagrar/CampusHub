namespace CampusHub.Services
{
    public static class RoleService
    {
        public const string Admin = "Admin";
        public const string Teacher = "Teacher";
        public const string Student = "Student";

        public static readonly string[] AllRoles =
        {
            Admin,
            Teacher,
            Student
        };

        public static bool IsValidRole(string role)
        {
            return AllRoles.Contains(role);
        }
    }
}