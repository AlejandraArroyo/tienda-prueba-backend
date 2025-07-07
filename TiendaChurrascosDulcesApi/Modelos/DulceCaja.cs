using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("dulce_caja")]
    public class DulceCaja
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("dulce_id")]
        public int DulceId { get; set; }

     
        public Dulce Dulce { get; set; }

        [Column("cantidad_unidades")]
        public int CantidadUnidades { get; set; }

        [Column("stock_cajas")]
        public int StockCajas { get; set; }

        [Column("precio_caja")]
        public decimal PrecioCaja { get; set; }
    }
}
