using ImbaBetWeb.Models;

namespace ImbaBetWeb.Business.Helper
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
