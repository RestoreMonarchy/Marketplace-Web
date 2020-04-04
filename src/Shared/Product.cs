using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Marketplace.Shared
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Title { get; set; }
        [Required]
        [StringLength(2000)]
        public string Description { get; set; }
        [Required]
        [Range(0, 9999999)]
        public decimal Price { get; set; }
        public byte[] Icon { get; set; }
        [Required]
        public int MaxPurchases { get; set; }
        [Required]
        public bool Enabled { get; set; }

        [MinLength(1)]
        public virtual List<Server> Servers { get; set; }
        [MinLength(1)]
        public virtual List<Command> Commands { get; set; }
    }
}
