using ImbaBetWeb.Model;

namespace ImbaBetWeb.ViewModels.Orga
{
    public class MyCommunityViewModel
    {
        public required IEnumerable<Community> Communities { get; set; }

        public required Player Player { get; set; }
    }
}
