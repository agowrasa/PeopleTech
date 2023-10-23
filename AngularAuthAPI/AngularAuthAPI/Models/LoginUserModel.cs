using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularAuthAPI.Models
{
    [Table("[PTG].[Employee]")]
    public class LoginUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(MAX)")]
        public string Password { get; set; }

        // Other properties as needed
    }
}
