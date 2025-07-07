namespace TiendaChurrascosDulcesApi.Modelos
{
    public class ChurrascoDto
    {
        public int TipoCarneId { get; set; }
        public int TerminoCoccionId { get; set; }
        public int Porciones { get; set; }
        public int PorcionesExtra { get; set; }

        public List<PorcionGuarnicionDto> PorcionGuarniciones { get; set; }
    }
}
