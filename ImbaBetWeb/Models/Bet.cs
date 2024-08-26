using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImbaBetWeb.Models
{
    [Table("Bets")]
    public record Bet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Match")]
        public int MatchId { get; set; }
        public virtual Match Match { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int GoalsA { get; set; }

        public int GoalsB { get; set; }
        public int Points { get; set; }
    }
}
