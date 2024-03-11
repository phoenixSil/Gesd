using Gesd.Api.Context;
using Gesd.Api.Repositories.Contrats;
using Gesd.Api.Settings;

using Microsoft.EntityFrameworkCore.Storage;

namespace Gesd.Api.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IFileRepository _fileRepository;
        private IBlobRepository _blobRepository;
        private IEncryptedFileRepository _encryptedFileRepository;
        private IKeyStoreRepository _keyStoreRepository;

        private readonly GesdContext _context;
        private readonly FileSettings _fichierSetting;

        public UnitOfWork(GesdContext context, FileSettings fichierSetting)
        {
            _context = context;
            _fichierSetting = fichierSetting;
        }

        public async Task Enregistrer()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public IFileRepository FileRepository => _fileRepository ??= new FileRepository(_context);
        public IBlobRepository BlobRepository => _blobRepository ??= new BlobRepository(_fichierSetting);
        public IEncryptedFileRepository EncryptedFileRepository => _encryptedFileRepository ??= new EncryptedFileRepository(_context);
        public IKeyStoreRepository KeyStoreRepository => _keyStoreRepository ??= new KeyStoreRepository(_context);

    }
}
