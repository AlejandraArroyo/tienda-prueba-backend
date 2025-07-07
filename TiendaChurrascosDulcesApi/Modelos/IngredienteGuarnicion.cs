using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("ingrediente_guarnicion")]
    public class IngredienteGuarnicion
    {
        [Column("id")] public int Id { get; set; }
        [Column("nombre")] public string Nombre { get; set; } = string.Empty;
        [Column("stock")] public decimal StockLibras { get; set; }
        [Column("precio_unitario")] public decimal PrecioLibra { get; set; }
    }
}
