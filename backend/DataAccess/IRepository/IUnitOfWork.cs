using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;

namespace backend.DataAccess.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Hotel> Hotels { get; }
        Task SaveChanges();
    }
}