using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImbaBetWeb.Models
{
    [Table("Matches")]
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        [ForeignKey("TeamA")]
        public int? TeamATeamId { get; set; }
        public virtual Team? TeamA { get; set; }

        [ForeignKey("TeamB")]
        public int? TeamBTeamId { get; set; }
        public virtual Team? TeamB { get; set; }

        public string? AlternativeTeamAText { get; set; }

        public string? AlternativeTeamBText { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }

        public bool IsOver { get; set; }

        [Required]
        [ForeignKey("MatchGroup")]
        public int MatchGroupId { get; set; }
        public virtual MatchGroup MatchGroup { get; set; }

        public override string ToString()
        {
            return $"Match ({Id})";
        }
    }
}
