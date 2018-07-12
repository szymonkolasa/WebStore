using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebStore.Data.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }

        [DataType(DataType.Text)]
        public string Name { get; set; }

        public bool ShowOnBar { get; set; }

        public Category ParentCategory { get; set; }

        public byte[] Image { get; set; }

        public string ImageType { get; set; }

        public List<Product> Products { get; set; }
    }
}
