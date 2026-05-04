using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Agregator.Data
{
    public partial class CMessage
    {
        public CMessage()
        {
        }

        [Key]
        public int RowId { get; set; }
        public string Text { get; set; }
        public DateTime Created { get; set; }
        public Guid? SendFromId { get; set; }
        public virtual AgregatorUser SendFrom { set; get; }
        public Guid SendToId { get; set; }
        public virtual AgregatorUser SendTo { set; get; }

    }
}

