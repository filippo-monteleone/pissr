using BusinessLogicLayer.Dtos;

namespace RestAPI_Pissr.Models
{
    public class AddDispositivoRequest
    {
        public DispositivoTipo Tipo { get; set; }
    }

    public class UpdateDispositivoRequest
    {
        public DispositivoTipo Tipo { get; set; }
    }
}
