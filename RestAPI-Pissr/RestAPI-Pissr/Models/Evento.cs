using BusinessLogicLayer.Dtos;

namespace RestAPI_Pissr.Models
{
    public class AddEventoRequest
    {
        public EventoTipo Tipo { get; set; }
        public DateTime Tempo { get; set; }
        public string? Valore { get; set; }
    }
}
