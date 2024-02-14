using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using RestAPI_Pissr;
using RestAPI_Pissr.Models;

namespace ApiPissr.Controllers
{
    [ApiController]
    [Route("api/azienda-agricola")]
    public class AziendaAgricolaController : Controller
    {
        private readonly AziendaAgricolaService _BLL;
        private readonly BackgroundMqttClient _BackgroundMqttClient;

        public AziendaAgricolaController(IDataService<AziendaAgricolaDTO> BLL, BackgroundMqttClient backgroundMqttClient)
        {
            _BackgroundMqttClient = backgroundMqttClient;
            _BLL = BLL as AziendaAgricolaService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAziende()
        {
            return Ok(await _BLL.GetList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult> GetAzienda(int id)
        {
            var azienda = await _BLL.GetById(id);
            if (azienda is null)
            {
                return NotFound();
            }

            return Ok(azienda);
        }

        [HttpGet]
        [Route("{id:int}/campi")]
        public async Task<ActionResult> GetAziendaCampi(int id)
        {
            var campi = await _BLL.GetCampi(id);
            return Ok(new {campi, total = campi.Count()});
        }

        [HttpPost]
        public async Task<ActionResult> AddAzienda(AddAziendaAgricolaRequest nuovaAzienda)
        {

            var azienda = await _BLL.Insert(new AziendaAgricolaDTO()
            {
                Indirizzo = nuovaAzienda.Indirizzo,
                Nome = nuovaAzienda.Nome
            });

            return CreatedAtAction(nameof(GetAzienda), new { id = azienda.Id }, azienda);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateAzienda(int id, UpdateAziendaAgricolaRequest azienda)
        {
            var aziendaDaModificare = await _BLL.Update(new AziendaAgricolaDTO()
            {
                Id = id,
                Indirizzo = azienda.Indirizzo,
                Nome = azienda.Nome
            });

            if (aziendaDaModificare is null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        [Route("{id:int}/richieste")]
        public async Task<ActionResult> GetRichieste()
        {
            var proposte = await _BLL.GetRichieste(_BackgroundMqttClient.Date);

            return Ok(proposte);
        }

        [HttpGet]
        [Route("{id:int}/richiesta-corrente")]
        public async Task<ActionResult> GetRichiestaCorrente(int id)
        {
            var proposta = await _BLL.GetRichiesta(id, ContrattoStato.Accepted, _BackgroundMqttClient.Date);

            if (proposta is null)
            {
                return NotFound();
            }

            return Ok(proposta);
        }

        [HttpGet]
        [Route("{id:int}/proposta")]
        public async Task<ActionResult> GetProposta(int id)
        {
            var proposta = await _BLL.GetRichiesta(id, ContrattoStato.Pending, _BackgroundMqttClient.Date);
            
            if (proposta is null)
            {
                return NotFound();
            }
            
            return Ok(proposta);
        }

        [HttpPut]
        [Route("{id:int}/proposta")]
        public async Task<ActionResult> AddProposta(int id, AddContratto nuovoContratto)
        {
            var domani = _BackgroundMqttClient.Date.AddDays(1);
            domani = new DateTime(domani.Year, domani.Month, domani.Day, 0, 0, 0);

            var contratto = await _BLL.Insert(new ContrattoDTO()
            {
                Acqua = nuovoContratto.Acqua,
                Tempo = domani,
                AziendaAgricolaId = id,
                Stato = ContrattoStato.Pending,
                AziendaIdricaId = nuovoContratto.AziendaIdricaId
            });

            if (contratto is null)
                return UnprocessableEntity();

            return CreatedAtAction(nameof(GetProposta), new { id = contratto.Id}, contratto);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> DeleteAzienda(int id)
        {
            var aziendaDaEliminare = await _BLL.Delete(new AziendaAgricolaDTO() { Id = id });

            if (aziendaDaEliminare is null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
