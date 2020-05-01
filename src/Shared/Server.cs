using System.ComponentModel.DataAnnotations;
using System.Net.WebSockets;

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

        public virtual WebSocket WebSocket { get; set; }
        public virtual bool IsOnline => WebSocket?.CloseStatus.HasValue ?? false;
    }
}
