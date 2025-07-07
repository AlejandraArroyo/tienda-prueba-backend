using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    
        [Table("combo_churrasco")]
        public class ComboChurrasco
        {
            [Key]
        [Column("id")]
        public int Id { get; set; }

            [Column("combo_id")]
            public int ComboId { get; set; }
            public Combo? Combo { get; set; }

            [Column("churrasco_id")]
            public int ChurrascoId { get; set; }
            public Churrasco? Churrasco { get; set; }

            [Column("cantidad")]
            public int Cantidad { get; set; }
        
    }
}
