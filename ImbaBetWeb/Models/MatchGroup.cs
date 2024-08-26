using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImbaBetWeb.Models
{
    [Table("MatchGroups")]
    public class MatchGroup
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public bool HasGroupRanking { get; set; }

        public int StackRank { get; set; }

        public virtual IList<Match> Matches { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }

}
