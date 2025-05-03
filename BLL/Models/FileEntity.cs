using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class FileEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(256)]
        public string Name { get; set; } = default!;

        [Required]
        public string Path { get; set; } = default!;

        public long Size { get; set; }
        public DateTime LastModified { get; set; }

        public List<LineEntity> Lines { get; set; } = new();
    }
}
