using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Rocky_Model
{
    public class AplicationType
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Pick the Name")] 
        [Required]
        [StringLength(20, ErrorMessage = "Name length can't be more than 20.")]
        public string? Name { get; set; }

     
       

    }
}
