using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("combo_dulce_caja")]
    public class ComboDulceCaja
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("combo_id")]
        public int ComboId { get; set; }
        public Combo? Combo { get; set; }

        [Column("dulce_id")]
        public int DulceId { get; set; }
        public Dulce? Dulce { get; set; }

        [Column("tamaño_caja")]
        public int TamañoCaja { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}
