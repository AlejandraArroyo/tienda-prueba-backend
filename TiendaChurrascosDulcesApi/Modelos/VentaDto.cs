namespace TiendaChurrascosDulcesApi.Modelos
{
    public class VentaDto
    {
        public decimal Total { get; set; }
        public List<ItemComboDTO> Combos { get; set; } = new();
        public List<ItemChurrascoDTO> Churrascos { get; set; } = new();
    }
}
