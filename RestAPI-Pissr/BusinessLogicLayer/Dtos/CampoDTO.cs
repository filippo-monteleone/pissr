using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Dtos
{
    public record CampoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ushort Ettari { get; set; } = 0;
        public ushort Altitudine { get; set; } = 0;
        public bool Attivo { get; set; } = true;
        public bool Adattivo { get; set; } = false;

        public string TipoSistemaIrrigazione { get; set; } = String.Empty;
        public int Ae { get; set; } = 0;
        public float Fr { get; set; } = 0;
        public float S1 { get; set; } = 0;
        public float S2 { get; set; } = 0;

        public int Aws { get; set; } = 0;

        public double Rd { get; set; } = 0;
        public float Ac { get; set; } = 0;
        public double Hum { get; set; } = 0;
        public double Kc { get; set; } = 0;


        public double Sws => Rd * Aws;
        public double Mad => Sws * Ac;
        public double? SoilMoisture { get; set; } = null;
        public double? CriticalSoilMoisture => TipoSistemaIrrigazione switch
        {
            "Drip" => Sws - (Sws * 0.25f),
            "Sprinkler" => Sws - (Sws * 0.3f),
            _ => null
        };

        public int AziendaAgricolaId { get; set; }
    }
}
