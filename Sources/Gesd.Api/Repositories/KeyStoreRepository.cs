using Gesd.Api.Context;
using Gesd.Api.Repositories.Contrats;
using Gesd.Entite;

namespace Gesd.Api.Repositories
{
    public class KeyStoreRepository : GenericRepository<KeyStore>, IKeyStoreRepository
    {
        public KeyStoreRepository(GesdContext context) : base(context)
        { }
    }
}
