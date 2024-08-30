using ImbaBetWeb.Models;

namespace ImbaBetWeb.Logic.Helper
{
    public static class FlagHelpers
    {
        public static string GetSmallFlagPath(Team? team)
        {
            return "/Resources/Flags/Small/" + (team?.FlagCountryCode ?? "empty") + ".png"; ;
        }

        public static string GetLargeFlagPath(Team? team)
        {
            return "/Resources/Flags/Large/" + (team?.FlagCountryCode ?? "empty") + ".png";
        }
    }
}
