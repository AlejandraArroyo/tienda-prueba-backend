using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("venta")]
    public class Venta
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("fecha")]
        public DateTime FechaHora { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        public List<VentaDetalle> Detalles { get; set; } = new();
    }
}
