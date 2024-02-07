using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using Data_Access_Layer.Entities;
using Data_Access_Layer;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestAPI_Pissr.Models;

namespace ApiPissr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispositivoController : Controller
    {
        private readonly DispositivoService _BLL;

        public DispositivoController(IDataService<DispositivoDTO> BLL)
        {
            _BLL = (DispositivoService) BLL;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllDispositivi()
        {
            var dispositivi = await _BLL.GetList();
            return Ok(dispositivi);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult> GetDispositivo(int id)
        {
            var dispositivo = await _BLL.GetById(id);
            return Ok();
        }

        [HttpGet]
        [Route("{id:int}/[action]")]
        public async Task<ActionResult> GetEventi(int id)
        {
            var eventi = await _BLL.GetEventi(id);
            return Ok(eventi);
        }

        [HttpGet]
        [Route("{id:int}/[action]/{eventoId:int}")]
        public async Task<ActionResult> GetEvento(int id, int eventoId)
        {
            var evento = await _BLL.GetEvento(id, eventoId);
            return Ok(evento);
        }

        [HttpPost]
        [Route("{id:int}/[action]")]
        public async Task<ActionResult> AddEvento(int id, AddEventoRequest nuovoEvento)
        {
            var dispositivo = await _BLL.GetById(id);

            if (dispositivo is null)
                return UnprocessableEntity();

            var evento = await _BLL.Insert(new EventoDto()
            {
                Tipo = nuovoEvento.Tipo,
                Tempo = nuovoEvento.Tempo,
                Valore = nuovoEvento.Valore,
                DispositivoId = id,
                CampoId = dispositivo.CampoId
            });

            if (evento is null)
                return UnprocessableEntity();

            return CreatedAtAction(nameof(GetEvento), new { id, eventoId = evento.Id }, evento);
        }

        [HttpPut]
        [Route("{id:int}/[action]/{eventoId:int}")]
        public async Task<ActionResult> UpdateEvento(int id, int eventoId ,AddEventoRequest nuovoEvento)
        {
            var dispositivo = await _BLL.GetById(id);

            if (dispositivo is null)
                return UnprocessableEntity();

            var evento = await _BLL.Update(new EventoDto()
            {
                Id = eventoId,
                Tipo =  nuovoEvento.Tipo,
                Tempo = nuovoEvento.Tempo,
                Valore = nuovoEvento.Valore,
                DispositivoId = id,
                CampoId = dispositivo.CampoId
            });
            
            if (evento is null)
                return NotFound();

            return CreatedAtAction(nameof(GetEvento), new { id, eventoId = evento.Id  }, evento);
        }
    }
}
