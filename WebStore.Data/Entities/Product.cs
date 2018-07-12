using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entities
{
    public class Product
    {
        [Key]
        public Guid Id { get; set; }

        public Category Category { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public string ShortDescription { get; set; }

        public string Description { get; set; }

        public List<ProductImage> Images { get; set; }

        public DateTime CreationDate { get; set; }

        public byte[] Image { get; set; }

        public string ImageType { get; set; }

        public bool IsDeleted { get; set; }
    }
}
