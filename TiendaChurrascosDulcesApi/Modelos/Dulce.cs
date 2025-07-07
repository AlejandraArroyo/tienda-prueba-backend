using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("dulce")]
    public class Dulce
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("disponible")]
        public bool Disponible { get; set; }

        public List<Presentacion> Presentaciones { get; set; } = new();

    }
}
