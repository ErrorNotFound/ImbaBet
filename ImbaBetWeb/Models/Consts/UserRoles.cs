namespace ImbaBetWeb.Models.Consts
{
    public static class UserRoles
    {
        public const string Admin = "Admin";
        public const string Editor = "Editor";

        public static IEnumerable<string> AllRoles => [Admin, Editor];
    }
}
