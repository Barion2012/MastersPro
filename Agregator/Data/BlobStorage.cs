using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Agregator.Data
{
    public class BlobStorage
    {
        public int RowId { set; get; }

        public Guid? AppUserId { set; get; }
        public virtual AgregatorUser AppUser { set; get; }
        public Guid RowGuid { set; get; }
        public string Name { get; set; }
        public byte[] Blob { get; set; }
        public int Type { get; set; }
        public DateTime Created { get; set; }
        public bool IsMain { get; set; }
        public bool IsZip { get; set; }

    }
}
