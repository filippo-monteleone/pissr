namespace RestAPI_Pissr.Models
{
    public record AddCampoRequest
    {
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

        public int AziendaAgricolaId { get; set; }
    }

    public record UpdateCampoRequest
    {
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
        public int Hum { get; set; } = 0;
        public double Kc { get; set; } = 0;
    }
}
