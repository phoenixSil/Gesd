using Gesd.Api.Context;
using Gesd.Api.Repositories.Contrats;
using Gesd.Entite;

namespace Gesd.Api.Repositories
{
    public class EncryptedFileRepository : GenericRepository<EncryptedUrlFile>, IEncryptedFileRepository
    {
        public EncryptedFileRepository(GesdContext context) : base(context)
        { }
    }
}