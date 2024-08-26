using System.ComponentModel.DataAnnotations;

namespace ImbaBetWeb.Models
{
    public record Setting
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
        public string Default { get; set; }
        public string Description { get; set; }
    }
}
