using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Orga
{
    public class MyCommunityViewModel
    {
        public required IList<Community> Communities { get; set; }

        public required Player Player { get; set; }

        public required Community? CommunityOfPlayer { get; set; }
    }
}
