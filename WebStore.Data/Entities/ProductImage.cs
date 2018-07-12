using System;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entities
{
    public class ProductImage
    {
        [Key]
        public Guid Id { get; set; }

        public byte[] Image { get; set; }

        public string ImageType { get; set; }
    }
}
