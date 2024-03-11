using Gesd.Api.Dtos;
using Gesd.Api.Features.Communs;

namespace Gesd.Api.Features.Commands
{
    public class AddFileCmd : BaseComputeCmd
    {
        public FileToAddDto FileDto { get; set; }
    }

    public class AddFileWithoutDatabaseCmd : BaseComputeCmd
    {
        public FileToAddDto FileDto { get; set; }
    }
}
