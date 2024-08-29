using System.ComponentModel;

namespace ImbaBetWeb.Models.Consts
{
    public class SettingNames
    {
        [Description("Name of this game.")]
        [DefaultValue("ImbaBet")]
        public const string BETTING_GAME_NAME = "BETTING_GAME_NAME";

        [Description("How many members are needed for community to be part of ranking.")]
        [DefaultValue("3")]
        public const string MIN_MEMBER_COUNT_FOR_RANKING = "MIN_MEMBER_COUNT_FOR_RANKING";
        
        [Description("When player bets the exact result.")]
        [DefaultValue("4")]
        public const string BETTING_POINTS_EXACT_RESULT = "BETTING_POINTS_EXACT_RESULT";

        [Description("When player bets the right winner and goal difference.")]
        [DefaultValue("3")]
        public const string BETTING_POINTS_CORRECT_TENDENCY_AND_DIFFERENCE = "BETTING_POINTS_CORRECT_TENDENCY_AND_DIFFERENCE";

        [Description("When player bets the right winner.")]
        [DefaultValue("2")]
        public const string BETTING_POINTS_CORRECT_TENDENCY = "BETTING_POINTS_CORRECT_TENDENCY";

        [Description("Communities can be created.")]
        [DefaultValue("true")]
        public const string ALLOW_COMMUNITY_CREATE = "ALLOW_COMMUNITY_CREATE";

        [Description("Communities can be joined.")]
        [DefaultValue("true")]
        public const string ALLOW_COMMUNITY_JOIN = "ALLOW_COMMUNITY_JOIN";

        [Description("Communities can be left.")]
        [DefaultValue("true")]
        public const string ALLOW_COMMUNITY_LEAVE = "ALLOW_COMMUNITY_LEAVE";

        [Description("How many times can a user change his username.")]
        [DefaultValue("2")]
        public const string USERNAME_RENAME_LIMIT = "USERNAME_RENAME_LIMIT";

        [Description("How many points a team gains for a match win.")]
        [DefaultValue("3")]
        public const string MATCH_POINTS_PER_WIN = "MATCH_POINTS_PER_WIN";

        [Description("How many points a team gains for a match draw.")]
        [DefaultValue("1")]
        public const string MATCH_POINTS_PER_DRAW = "MATCH_POINTS_PER_DRAW";
    }
}
