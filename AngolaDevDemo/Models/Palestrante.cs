using System.ComponentModel.DataAnnotations;

namespace AngolaDevDemo.Models
{
    public class Palestrante
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Nome { get; set; }
        public string? Tema { get; set; }
    }
}
