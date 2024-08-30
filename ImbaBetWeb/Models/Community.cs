using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImbaBetWeb.Models
{
    [Table("Communities")]
    public class Community
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }

        public required string OwnerId { get; set; }
        
        public virtual ApplicationUser Owner { get; set; } = null!;

        public virtual IList<ApplicationUser> Members { get; set; } = null!;

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
