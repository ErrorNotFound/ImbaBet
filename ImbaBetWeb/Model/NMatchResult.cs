using ImbaBetWeb.Models;

namespace ImbaBetWeb.Model
{
    public class NMatchResult
    {
        public bool IsDraw { get; set; }
        public NTeam? Winner { get; set; }
    }
}
