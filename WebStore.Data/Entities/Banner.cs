using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entities
{
    public class Banner
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }

        public byte[] Image { get; set; }

        public string ImageType { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        public ApplicationUser Creator { get; set; }

        public string Link { get; set; }
    }
}
