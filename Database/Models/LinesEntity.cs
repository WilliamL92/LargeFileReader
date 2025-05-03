using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Database.Models
{
    public class LineEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(FileEntity))]
        public int IdFile { get; set; }

        [Required, MaxLength(100)]
        public string Text { get; set; } = default!;

        [Required]
        public int Position { get; set; }
        public FileEntity? FileEntity { get; set; }
    }
}
