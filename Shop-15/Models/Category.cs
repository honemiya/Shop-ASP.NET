using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Shop_15.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [DisplayName("Show order")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Show order must be more then 0")]
        public int ShowOrder { get; set; }
    }
}
