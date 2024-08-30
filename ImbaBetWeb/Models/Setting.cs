using System.ComponentModel.DataAnnotations;

namespace ImbaBetWeb.Models
{
    public record Setting
    {
        [Key]
        public required string Key { get; set; }
        public required string Value { get; set; }
        public required string Default { get; set; }
        public required string Description { get; set; }
    }
}
