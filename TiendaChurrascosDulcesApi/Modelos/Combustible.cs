using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("combustible")]
    public class Combustible
    {
        [Column("id")] public int Id { get; set; }
        [Column("tipo")] public string Tipo { get; set; } = string.Empty;
        [Column("stock_libras")] public decimal StockLibras { get; set; }
        [Column("precio_libra")] public decimal PrecioLibra { get; set; }
        [Column("descripcion")] public string? Descripcion { get; set; }
    }
}
