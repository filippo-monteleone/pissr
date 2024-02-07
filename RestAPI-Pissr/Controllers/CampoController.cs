
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using Data_Access_Layer;
using Data_Access_Layer.Entities;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using RestAPI_Pissr.Models;

namespace ApiPissr.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampoController : Controller
    {
        private readonly CampoService _BLL;

        public CampoController(IDataService<CampoDTO> bll)
        {
            _BLL = (CampoService) bll;
        }

        [HttpGet]
        public async Task<ActionResult> GetCampi()
        {
            var campi = await _BLL.GetList();
            return Ok(campi);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult> GetCampo(int id)
        {
            var campo = await _BLL.GetById(id);

            if (campo is null)
            {
                return NotFound();
            }

            return Ok(campo);
        }

        [HttpGet]
        [Route("{id:int}/[action]")]
        public async Task<ActionResult> GetDispositivi(int id)
        {
            var dispositivi = await _BLL.GetDispositivi(id);

            return Ok(dispositivi);
        }

        [HttpPost]
        [Route("{id:int}/[action]")]
        public async Task<ActionResult> AddDispositivo(int id, AddDispositivoRequest nuovoDispositivo)
        {
            var campo = await _BLL.GetById(id);

            if (campo is null)
            {
                return UnprocessableEntity();
            }

            var dispositivo = await _BLL.Insert(new DispositivoDTO()
            {
                Tipo = nuovoDispositivo.Tipo,
                CampoId = campo.Id,
            });

            if (dispositivo is null)
                return UnprocessableEntity();

            return CreatedAtAction(nameof(DispositivoController.GetDispositivo), new { Controller = "Dispositivo", id = dispositivo.Id }, dispositivo);
        }

        [HttpDelete]
        [Route("{id:int}/[action]")]
        public async Task<IActionResult> DeleteDispositivo(int id)
        {
            var dispositivoDaEliminare = await _BLL.Delete(new DispositivoDTO() { Id = id });

            if (dispositivoDaEliminare is null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet]
        [Route("{id:int}/Irrigazione")]
        public async Task<ActionResult> GetIrrigazione(int id, DateTime data)
        {

            var (max, min) = await _BLL.GetTemp(id, data);
            var rain = await _BLL.GetRain(id, data);
            var rad = await _BLL.GetMaxRad(id, data);
            var humRimasta = await _BLL.UmiditaRimasta(id, data);

            var etc = _BLL.GetEtc(id, data);

            return Ok(new
            {
                max,
                min,
                rain,
                rad,
                humRimasta,
                etc
            });
        }

        [HttpPost]
        public async Task<ActionResult> AddCampo(AddCampoRequest nuovoCampo)
        {
            var campo = await _BLL.Insert(new CampoDTO()
            {
                Nome = nuovoCampo.Nome,
                Ettari = nuovoCampo.Ettari,
                Altitudine = nuovoCampo.Altitudine,
                Attivo = nuovoCampo.Attivo,
                Adattivo = nuovoCampo.Adattivo,
                TipoSistemaIrrigazione = nuovoCampo.TipoSistemaIrrigazione,
                Ae = nuovoCampo.Ae,
                Fr = nuovoCampo.Fr,
                S1 = nuovoCampo.S1,
                S2 = nuovoCampo.S2,
                Aws = nuovoCampo.Aws,
                Rd = nuovoCampo.Rd,
                Ac = nuovoCampo.Ac,
                Hum = nuovoCampo.Hum,
                Kc = nuovoCampo.Kc,
                AziendaAgricolaId = nuovoCampo.AziendaAgricolaId,
            });

            if (campo == null)
                return UnprocessableEntity();

            return CreatedAtAction(nameof(GetCampo), new { id = campo.Id }, campo);
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateCampo(int id, UpdateCampoRequest campoModificato)
        {
            var campoDaModificare = await _BLL.Update(new CampoDTO()
            {
                Id = id,
                Nome = campoModificato.Nome,
                Ettari = campoModificato.Ettari,
                Altitudine = campoModificato.Altitudine,
                Attivo = campoModificato.Attivo,
                Adattivo = campoModificato.Adattivo,
                TipoSistemaIrrigazione = campoModificato.TipoSistemaIrrigazione,
                Ae = campoModificato.Ae,
                Fr = campoModificato.Fr,
                S1 = campoModificato.S1,
                S2 = campoModificato.S2,
                Aws = campoModificato.Aws,
                Rd = campoModificato.Rd,
                Ac = campoModificato.Ac,
                Hum = campoModificato.Hum,
                Kc = campoModificato.Kc
            });

            if (campoDaModificare is null)
            {
                return NotFound();
            }

            return NoContent();

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteCampo(int id)
        {
            var campoDaEliminare = await _BLL.Delete(new CampoDTO() { Id = id });

            if (campoDaEliminare is null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
