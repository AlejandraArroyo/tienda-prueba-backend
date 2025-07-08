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

        [Column("tipo_producto")]
        public string TipoProducto { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        public Venta? Venta { get; set; }
        [Column("subtotal")]
        public decimal PrecioUnitario { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

    }
}
