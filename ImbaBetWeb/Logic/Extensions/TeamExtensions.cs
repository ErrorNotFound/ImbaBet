using ImbaBetWeb.Models;

namespace ImbaBetWeb.Logic.Extensions
{
    public static class TeamExtensions
    {
        public static string GetSmallFlagPath(this Team team)
        {
			return "/Resources/Flags/Small/" + (team.FlagCountryCode ?? "empty") + ".png"; ;
        }

        public static string GetLargeFlagPath(this Team team)
        {
            return "/Resources/Flags/Large/" + (team.FlagCountryCode ?? "empty") + ".png";
        }
    }
}
