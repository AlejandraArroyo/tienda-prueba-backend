using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("churrasco")]
    public class Churrasco
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("tipo_carne_id")]
        public int TipoCarneId { get; set; }
        public TipoCarne TipoCarne { get; set; }

        [Column("termino_coccion_id")]
        public int TerminoCoccionId { get; set; }
        public TerminoCoccion TerminoCoccion { get; set; }

        [Column("porciones")]
        public int Porciones { get; set; }

        [Column("porciones_extra")]
        public int PorcionesExtra { get; set; }

    
        public ICollection<ChurrascoGuarnicion>? ChurrascosGuarnicion { get; set; }
    }
}
