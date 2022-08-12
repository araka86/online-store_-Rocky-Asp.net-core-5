using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Rocky_Model
{
    public class Product
    {
        public Product() => TempSqFt = 1;
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? shortDesc { get; set; }
        public string? Description { get; set; }
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public string? Image { get; set; }
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
        [Display(Name = "Aplication Type")]
        public int AplicationTypeId { get; set; }
        [ForeignKey("AplicationTypeId")]
        public virtual AplicationType? AplicationType { get; set; }
        [NotMapped]
        [Range(1, 10000,ErrorMessage ="Sqft must be gereater than 0.")]
        public int TempSqFt { get; set; } //кол-тво товара      
    }
}
