using DataAccessLayer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        protected readonly ApiDbContext _dbContext;
        public IGenericRepository<AziendaAgricola> AziendeAgricole { get; }
        public IGenericRepository<AziendaIdrica> AziendeIdriche { get; }
        public IGenericRepository<Campo> Campi { get; }
        public IGenericRepository<Dispositivo> Dispositivi { get; }
        public IGenericRepository<Evento> Eventi { get; }
        public IGenericRepository<Contratto> Contratti { get;  }

        public UnitOfWork(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
            AziendeAgricole = new GenericRepository<AziendaAgricola>(dbContext);
            AziendeIdriche = new GenericRepository<AziendaIdrica>(dbContext);
            Campi = new GenericRepository<Campo>(dbContext);
            Dispositivi = new GenericRepository<Dispositivo>(dbContext);
            Eventi = new GenericRepository<Evento>(dbContext);
            Contratti = new GenericRepository<Contratto>(dbContext);
        }

        public int Complete()
        {
            var res = _dbContext.SaveChanges();
            return res;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public void Clear()
        {
            _dbContext.ChangeTracker.Clear();
        }
    }
}
