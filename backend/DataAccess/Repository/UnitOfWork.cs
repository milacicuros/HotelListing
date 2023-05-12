using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using backend.Data;
using backend.DataAccess.IRepository;

namespace backend.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private IGenericRepository<Country> _countries;
        private IGenericRepository<Hotel> _hotels;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        // obj ??= means if obj is null then do something
        public IGenericRepository<Country> Countries => _countries ??= new GenericRepository<Country>(_context);

        public IGenericRepository<Hotel> Hotels => _hotels ??= new GenericRepository<Hotel>(_context);


        public void Dispose()
        {
            _context.Dispose();
            // GC - garbage collector
            GC.SuppressFinalize(this);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}