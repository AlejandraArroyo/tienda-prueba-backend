namespace TiendaChurrascosDulcesApi.Modelos
{
    public class ComboDto
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public List<ChurrascoCantidadDto> Churrascos { get; set; }
        public List<DulceUnidadCantidadDto> DulcesUnidad { get; set; }
        public List<DulceCajaCantidadDto> DulcesCaja { get; set; }
    }
}
