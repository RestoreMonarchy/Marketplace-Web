using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Marketplace.Shared
{
    public class Command
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string CommandName { get; set; }
        [Required]
        [StringLength(255)]
        public string CommandHelp { get; set; }
        [Required]
        [StringLength(255)]
        public string CommandText { get; set; }
        public bool Expires { get; set; }
        [Range(0, int.MaxValue)]
        public int ExpireTime { get; set; }
        [StringLength(255)]
        public string ExpireCommand { get; set; }
        [Required]
        public bool ExecuteOnBuyerJoinServer { get; set; }
    }
}
