using System.Drawing;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Mapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BusinessLogicLayer.Services;

public class AziendaIdricaService : IDataService<AziendaIdricaDTO>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AutoMapper.Mapper mapper;

    public AziendaIdricaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        mapper = MapperConfig.InitializeAutomapper();
    }
    public async Task<IEnumerable<AziendaIdricaDTO>> GetList()
    {
        var aziende = await _unitOfWork.AziendeIdriche.GetAll();

        return mapper.Map<IEnumerable<AziendaIdrica>, IEnumerable<AziendaIdricaDTO>>(aziende);
    }

    /*
     * Resituisco le richieste in attesa di una certa data e se ci sono alcune che
     * sono scadute vengono segnate come tali su db
     */
    private async Task CheckPendingRichieste(DateTime now)
    {
        var proposte = from contratto in await _unitOfWork.Contratti.GetAll()
            where contratto.Stato == ContrattoStato.Pending
            select contratto;

        Contratto propostaValida = null;

        foreach (var proposta in proposte)
        {
            if (proposta.Tempo <= now)
            {
                proposta.Stato = ContrattoStato.Expired;
            }
            else
            {
                propostaValida = proposta;
            }
        }

        _unitOfWork.Complete();

    }

    public async Task<IEnumerable<ContrattoDTO>> GetRichieste(DateTime now)
    {
        await CheckPendingRichieste(now);

        var richieste = await _unitOfWork.Contratti.GetAll();

        return mapper.Map<IEnumerable<ContrattoDTO>>(richieste);
    }

    public async Task<AziendaIdricaDTO?> GetById(int id)
    {
        var azienda = await _unitOfWork.AziendeIdriche.GetById(id);

        return mapper.Map<AziendaIdricaDTO>(azienda);
    }

    public async Task<AziendaIdricaDTO?> Insert(AziendaIdricaDTO entity)
    {
        var nuovaAzienda = await _unitOfWork.AziendeIdriche.Insert(mapper.Map<AziendaIdrica>(entity));

        _unitOfWork.Complete();

        return mapper.Map<AziendaIdricaDTO>(nuovaAzienda);
    }

    public async Task<ContrattoDTO> Insert(ContrattoDTO entity)
    {
        var nuovoContratto = await _unitOfWork.Contratti.Insert(mapper.Map<Contratto>(entity));

        _unitOfWork.Complete();

        return mapper.Map<ContrattoDTO>(nuovoContratto);
    }

    public async Task<AziendaIdricaDTO?> Update(AziendaIdricaDTO entity)
    {
        var modificataAzienda = await _unitOfWork.AziendeIdriche.Update(mapper.Map<AziendaIdrica>(entity));

        _unitOfWork.Complete();

        return mapper.Map<AziendaIdricaDTO>(modificataAzienda);
    }

    public async Task<ContrattoDTO?> Update(ContrattoDTO entity, DateTime now)
    {

        await CheckPendingRichieste(now);

        var contratto = await _unitOfWork.Contratti.GetById(entity.Id);
        
        if (contratto is null)
            return null;

        switch ((contratto.Stato, entity.Stato))
        {
            case (ContrattoStato.Pending, ContrattoStato.Accepted):
                // Setto il tempo di fine del vecchio contratto accettato
                var accContratto = (from contr in await _unitOfWork.Contratti.GetAll()
                    where contr.Stato == ContrattoStato.Accepted
                    orderby contr.Tempo descending 
                    select contr).FirstOrDefault();
                if (accContratto is not null)
                    accContratto.Fine = contratto.Tempo;
                contratto.Stato = entity.Stato;
                contratto.Acqua = entity.Acqua;
                break;
            case (ContrattoStato.Accepted, ContrattoStato.Accepted):
                // Cambio i parametri del contratto
                contratto.Stato = entity.Stato;
                contratto.Acqua = entity.Acqua;
                break;
            case (ContrattoStato.Pending, ContrattoStato.Refused):
                contratto.Stato = entity.Stato;
                break;
        }


        _unitOfWork.Complete();

        return mapper.Map<ContrattoDTO>(contratto);
    }

    public async Task<AziendaIdricaDTO?> Delete(AziendaIdricaDTO entity)
    {
        var eliminataAzienda = await _unitOfWork.AziendeIdriche.Delete(mapper.Map<AziendaIdrica>(entity));

        _unitOfWork.Complete();

        return mapper.Map<AziendaIdricaDTO>(entity);
    }

    public Task<AziendaIdricaDTO> Get(AziendaIdricaDTO entity)
    {
        throw new NotImplementedException();
    }
}