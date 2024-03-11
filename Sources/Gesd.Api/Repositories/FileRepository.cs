using Gesd.Api.Context;
using Gesd.Api.Repositories.Contrats;

using Microsoft.EntityFrameworkCore;

using File = Gesd.Entite.File;

namespace Gesd.Api.Repositories
{
    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        public FileRepository(GesdContext context) : base(context)
        { }

    }
}
