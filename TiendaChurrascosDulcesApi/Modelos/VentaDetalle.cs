using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("venta_detalle")]
    public class VentaDetalle
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("combo_id")]
        public int? ComboId { get; set; } 

        [Column("churrasco_id")]
        public int? ChurrascoId { get; set; }  

        [Column("cantidad")]
        public int Cantidad { get; set; }

        public Venta? Venta { get; set; }
        public Combo? Combo { get; set; }
        public Churrasco? Churrasco { get; set; }
    }
}
