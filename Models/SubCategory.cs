using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SalesApp.Models
{
    public class SubCategory
    {
        public int Id { get; set; }
        public int  CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string DateCreated { get; set; }

    }
}
