using System.ComponentModel.DataAnnotations;

namespace AspCoreMvcWithAdo_CRUD.Models
{
    public class Employee
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(16, int.MaxValue,ErrorMessage ="Should be greater than | 12")]
        public int Age { get; set; }
        public Boolean IsActive { get; set; }
    }
}
