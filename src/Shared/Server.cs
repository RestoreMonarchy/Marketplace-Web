using System.ComponentModel.DataAnnotations;

namespace Marketplace.Shared
{
    public class Server
    {        
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string ServerName { get; set; }
        [Required]
        [StringLength(255)]
        public string ServerIP { get; set; }
        [Required]        
        public int? ServerPort { get; set; }
        public bool Enabled { get; set; }
    }
}
