using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImbaBetWeb.Models
{
    [Table("Communities")]
    public class Community
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public string OwnerId { get; set; }
        
        public virtual ApplicationUser Owner { get; set; }

        public virtual IList<ApplicationUser> Members { get; set; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
