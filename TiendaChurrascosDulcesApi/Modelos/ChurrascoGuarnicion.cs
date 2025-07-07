using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TiendaChurrascosDulcesApi.Modelos
{
    [Table("churrasco_guarnicion")]
    public class ChurrascoGuarnicion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("churrasco_id")]
        public int ChurrascoId { get; set; }

        [Column("guarnicion_id")]
        public int GuarnicionId { get; set; }

        [Column("porcion_numero")]
        public int PorcionNumero { get; set; }

        [ForeignKey("ChurrascoId")]
        public Churrasco? Churrasco { get; set; }

        [ForeignKey("GuarnicionId")]
        public Guarnicion? Guarnicion { get; set; }
    }


}
