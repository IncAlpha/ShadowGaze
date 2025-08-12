using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShadowGaze.Data.Models.Database;

public abstract class BaseDatabaseModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
        
    // public DateTime CreatedAt { get; set; } = DateTime.Now;
}