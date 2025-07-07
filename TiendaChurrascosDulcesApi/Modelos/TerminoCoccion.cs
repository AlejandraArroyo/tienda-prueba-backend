using System.ComponentModel.DataAnnotations.Schema;
namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("termino_coccion")]
    public class TerminoCoccion
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("descripcion")]
        public string Descripcion { get; set; }
    }
}
