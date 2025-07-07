namespace TiendaChurrascosDulcesApi.Modelos
{
    public class VentaDto
    {
        public decimal Total { get; set; }
        public List<VentaComboDto> Combos { get; set; } = new();
        public List<VentaChurrascoDto> Churrascos { get; set; } = new();
    }
}
