using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("dulce_unidad")]
    public class DulceUnidad
    {
        [Column("id")] public int Id { get; set; }
        [Column("dulce_id")]
        public int DulceId { get; set; }
        [Column("stock_unidades")] public int StockUnidades { get; set; }
        [Column("precio_unitario")] public decimal PrecioUnitario { get; set; }

        public Dulce Dulce { get; set; }
    }
}
