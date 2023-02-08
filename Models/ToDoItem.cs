using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public class ToDoItem
    {

        public int Id { get; set; }

        [Required]
        public string? AppUserId { get; set; }

        [Required]
        [Display(Name = "Item Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and max {1} characters long.", MinimumLength = 2)]
        
        public string? Name { get; set; }

        [Required]
        [DataType(DataType.Date)]

        public DateTime DateCreated { get; set; }


        
        [DataType(DataType.Date)]

        public DateTime? DueDate { get; set; }


        [Required] 
        public bool Completed { get; set; } 


        //Navigation Properties

        public virtual AppUser? AppUser { get; set; }

        public virtual ICollection<Accessory> Accessories { get; set; } = new HashSet<Accessory>();


    }
}
