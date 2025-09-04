using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//CE FICHIER SERT DE MAPPING DE LA BDD

namespace Backend.Models
{
    [Table("USERS")]
    public class User
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("USERNAME")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("EMAIL")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Column("FIRST_NAME")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Column("LAST_NAME")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}