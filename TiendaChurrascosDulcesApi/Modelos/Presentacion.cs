using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("presentacion")]
    public class Presentacion
    {

        [Column("id")]
        public int Id { get; set; }

        [Column("dulce_id")]
        public int DulceId { get; set; }

        [Column("tipo_venta")]
        public string TipoVenta { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [JsonIgnore] 
        public Dulce? Dulce { get; set; }
    }
}
