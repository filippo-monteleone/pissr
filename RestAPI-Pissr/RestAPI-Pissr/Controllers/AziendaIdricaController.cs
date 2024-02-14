using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using Data_Access_Layer.Entities;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_Pissr;
using RestAPI_Pissr.Models;

namespace ApiPissr.Controllers
{
    [ApiController]
    [Route("api/azienda-idrica")]
    public class AziendaIdricaController : Controller
    {
        private readonly AziendaIdricaService _BLL;
        private readonly BackgroundMqttClient _BackgroundMqttClient;

        public AziendaIdricaController(IDataService<AziendaIdricaDTO> BLL, BackgroundMqttClient backgroundMqttClient)
        {
            _BLL = (AziendaIdricaService)BLL;
            _BackgroundMqttClient = backgroundMqttClient;
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

        [HttpPost]
        public async Task<ActionResult> AddAzienda(AddAziendaIdricaRequest nuovaAzienda)
        {

            var azienda = await _BLL.Insert(new AziendaIdricaDTO()
            {
                Indirizzo = nuovaAzienda.Indirizzo,
                Nome = nuovaAzienda.Nome,
            });

            return CreatedAtAction(nameof(GetAzienda), new { id = azienda.Id }, azienda);
        }

        [HttpGet]
        [Route("{id:int}/richieste")]
        public async Task<ActionResult> GetRichieste()
        {
            var richieste = await _BLL.GetRichieste(_BackgroundMqttClient.Date);

            return Ok(richieste);
        }

        [HttpPut]
        [Route("{id:int}/richieste/{richiestaId:int}")]
        public async Task<ActionResult> UpdateRichiesta(int id, int richiestaId,  UpdateContratto modificatoContratto)
        {
            var richiesta = await _BLL.Update(new ContrattoDTO()
            {
                Id = richiestaId,
                AziendaIdricaId = id,
                Stato = modificatoContratto.Stato,
                Acqua = modificatoContratto.Acqua,
            }, _BackgroundMqttClient.Date);

            return Ok(richiesta);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult> UpdateAzienda(int id, UpdateAziendaIdricaRequest azienda)
        {
            var aziendaDaModificare = await _BLL.Update(new AziendaIdricaDTO()
            {
                Id = id,
                Indirizzo = azienda.Indirizzo,
                Nome = azienda.Nome,
            });

            if (aziendaDaModificare is null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult> DeleteAzienda(int id)
        {
            var aziendaDaEliminare = await _BLL.Update(new AziendaIdricaDTO() { Id = id });

            if (aziendaDaEliminare is null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
