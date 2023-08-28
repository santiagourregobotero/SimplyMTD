using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimplyMTD.Models.MTD
{
    [Table("Planing", Schema = "dbo")]
    public partial class Planing
    {
        [Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public string Id { get; set; }

        public string Title { get; set; }

    }
}