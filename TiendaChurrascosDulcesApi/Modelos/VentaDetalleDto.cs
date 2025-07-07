namespace TiendaChurrascosDulcesApi.Modelos
{
    public class VentaDetalleDto
    {
        public string TipoProducto { get; set; } = ""; 
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}
