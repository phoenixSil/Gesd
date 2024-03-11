using AutoMapper;

using Gesd.Api.Features.Commands;
using Gesd.Api.Features.Communs;
using Gesd.Api.Repositories.Contrats;
using Gesd.Entite.Responses;

using MediatR;

namespace Gesd.Api.Features.Handlers
{
    public class TelechargerFichierCmdHandler : BaseComputeCmdHandler<TelechargerFichierCmd>
    {
        public TelechargerFichierCmdHandler(IMediator mediator, IUnitOfWork unitOfWork, IMapper mapper, ILogger<BaseComputeCmdHandler<TelechargerFichierCmd>> logger) : base(mediator, unitOfWork, mapper, logger)
        {
        }

        public override async Task<RequestResponse> Handle(TelechargerFichierCmd request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Debut de Traitement");
            try
            {
                _logger.LogInformation("Ajout du fichier dans le Blob");

                var fichier = await _unitOfWork.BlobRepository.TelechargerFichier(request.Url).ConfigureAwait(false);

                if (string.IsNullOrEmpty(fichier))
                {
                    return ApiResponse<IEnumerable<string>>.CreateNotFoundResponse("Empty Database");
                }

                return ApiResponse<string>.CreateSuccessResponse(fichier);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Une erreur est survenue lors de la lecture des fichiers {ex.Message}");
                return ApiResponse<string>.CreateErrorResponse(ex, "Un problème est survenu lors de la lecture des fichiers");
            }
        }
    }
}
