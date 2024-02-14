namespace BusinessLogicLayer.Dtos
{
    public enum EventoTipo
    {
        Temp,
        Rad,
        Hum,
        Rain,
        On,
        Off,
        Idle,
    }

    public record EventoDto
    {
        public int Id { get; set; }
        public EventoTipo Tipo { get; set; }
        public DateTime Tempo { get; set; }
        public string? Valore { get; set; }
        public int DispositivoId { get; set; }
        public int CampoId { get; set; }
    }
}
