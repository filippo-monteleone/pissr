using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Dtos
{
    public enum DispositivoTipo
    {
        Sensore,
        Attuatore
    }

    public record DispositivoDTO
    {
        public int Id { get; set; }
        public DispositivoTipo Tipo { get; set; }

        public int CampoId { get; set; }
    }
}
