using AutoMapper;

using Gesd.Api.Features.Commands;
using Gesd.Api.Features.Communs;
using Gesd.Api.Repositories.Contrats;
using Gesd.Entite.Responses;
using MediatR;

using File = Gesd.Entite.File;

namespace Gesd.Api.Features.Handlers
{
    public class GetFileCmdHandler : BaseComputeCmdHandler<GetFileCmd>
    {
        public GetFileCmdHandler(IMediator mediator, IUnitOfWork unitOfWork, IMapper mapper, ILogger<BaseComputeCmdHandler<GetFileCmd>> logger) : base(mediator, unitOfWork, mapper, logger)
        {
        }

        public async override Task<RequestResponse> Handle(GetFileCmd request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Debut de Traitement");
            try
            {
                _logger.LogInformation("Ajout du fichier dans le Blob");

                var listFichier = await _unitOfWork.BlobRepository.GetAllMetadata().ConfigureAwait(false);

                if (listFichier?.Any() != true)
                {
                    return ApiResponse<IEnumerable<File>>.CreateNotFoundResponse("Empty Database");
                }

                return ApiResponse<IEnumerable<File>>.CreateSuccessResponse(listFichier);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Une erreur est survenue lors de la lecture des fichiers {ex.Message}");
                return ApiResponse<IEnumerable<File>>.CreateErrorResponse(ex, "Un problème est survenu lors de la lecture des fichiers");
            }
        }
    }
}
