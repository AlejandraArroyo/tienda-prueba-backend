using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("proveedor")]
    public class Proveedor
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }

        [Column("categoria")]
        public string Categoria { get; set; }  
    }
}
