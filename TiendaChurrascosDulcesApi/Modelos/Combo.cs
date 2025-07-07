using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    public class Combo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        public ICollection<ComboChurrasco>? Churrascos { get; set; }
        public ICollection<ComboDulceUnidad>? DulcesUnidad { get; set; }
        public ICollection<ComboDulceCaja>? DulcesCaja { get; set; }
    }
}
