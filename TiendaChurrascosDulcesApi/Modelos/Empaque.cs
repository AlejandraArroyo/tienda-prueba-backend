using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("empaque")]
    public class Empaque
    {
        [Column("id")] public int Id { get; set; }
        [Column("tipo")] public string Tipo { get; set; } = string.Empty;
        [Column("capacidad_unidades")] public int CapacidadUnidades { get; set; }
        [Column("stock")] public int Stock { get; set; }
        [Column("precio")] public decimal Precio { get; set; }
    }
}
