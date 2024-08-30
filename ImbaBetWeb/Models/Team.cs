using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImbaBetWeb.Models
{
    [Table("Teams")]
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public string? FlagCountryCode { get; set; }

        public int StackRank { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
    }
}
