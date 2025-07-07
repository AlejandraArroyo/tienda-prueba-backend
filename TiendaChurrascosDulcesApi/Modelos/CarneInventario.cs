using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("carne")]
    public class CarneInventario
    {

        [Column("id")] public int Id { get; set; }
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;
        [Column("stock_libra")] public decimal StockLibras { get; set; }
        [Column("precio_libra")] public decimal PrecioLibra { get; set; }
        
    }
}
