using AutoMapper;
using Gesd.Api.Dtos.Validators;
using Gesd.Api.Features.Commands;
using Gesd.Api.Features.Communs;
using Gesd.Api.Features.Tools;
using Gesd.Api.Repositories.Contrats;
using Gesd.Entite.Responses;
using MediatR;
using System.Net;

using File = Gesd.Entite.File;

namespace Gesd.Api.Features.Handlers
{
    public class AddFileCmdHandler : BaseComputeCmdHandler<AddFileCmd>
    {
        public AddFileCmdHandler(IMediator mediator, IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddFileCmdHandler> logger)
            : base(mediator, unitOfWork, mapper, logger)
        {
        }

        public override async Task<RequestResponse> Handle(AddFileCmd request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Vérification de la Taille du fichier");
            VerifierLaTailleDuFichier(request.FileDto.File);

            _logger.LogInformation("Début de Traitement");

            var validator = new AddFileValidator();
            var result = await validator.ValidateAsync(request.FileDto, cancellationToken).ConfigureAwait(false);

            if (!result.IsValid)
            {
                return ApiResponse<File>.CreateErrorResponse(HttpStatusCode.InternalServerError, "Une erreur est survenue dans le serveur", result.Errors.Select(q => q.ErrorMessage).ToList());
            }

            string? url = null;
            string? fileName = null;
            int version = 1;

            try
            {
                _logger.LogInformation("Ajout du fichier dans le Blob");
                (fileName, url, version) = await _unitOfWork.BlobRepository.Add(request.FileDto.File).ConfigureAwait(false);

                _logger.LogInformation("Début de la transaction de sauvegarde ");
                await using var transaction = _unitOfWork.BeginTransaction();
                var dto = ComputeFile.GenererLeDtoDeSauvegarde(request.FileDto, fileName, version);
                var fileToSave = _mapper.Map<File>(dto);
                var blobdataResponse = await _unitOfWork.FileRepository.Add(fileToSave).ConfigureAwait(false);

                if (blobdataResponse == null)
                {
                    transaction.Rollback();
                    return ApiResponse<File>.CreateErrorResponse(HttpStatusCode.InternalServerError, "Une erreur est survenue dans le serveur", null);
                }

                _logger.LogInformation($"Sauvegarde des métadonnées URL:: {url}, BLOBDATARESPONSE:: {blobdataResponse}");
                await SauvegarderLesMetaData(url, blobdataResponse).ConfigureAwait(false);

                transaction.Commit();

                _logger.LogInformation("Sauvegarde terminée");

                return ApiResponse<File>.CreateSuccessResponse(fileToSave, "File added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Une erreur est survenue lors de la sauvegarde du fichier sélectionné {ex.Message}");
                if (url != null)
                {
                    // Rollback if the blob URL was generated but an exception occurred later
                    _logger.LogWarning($"Suppression du fichier sauvegardé précédemment, une erreur est survenue {url}");
                    _ = _unitOfWork.BlobRepository.Delete(url);
                }

                // Log exception here if needed
                return ApiResponse<File>.CreateErrorResponse(ex, "Un Problème est survenu lors de la sauvegarde du fichier");
            }
        }


        #region PRIVATE FUNCTION

        private async Task SauvegarderLesMetaData(string url, File blobdataResponse)
        {
            var cleDeChiffrement = ComputeFile.GenererCleDeChiffrement();
            var encryptedUrl = ComputeFile.EncryptagedeLUrl(url, cleDeChiffrement);
            var dtoEncryptedFile = ComputeFile.GenererDtoEncFile(blobdataResponse, encryptedUrl);
            var encUrlResponse = await _unitOfWork.EncryptedFileRepository.Add(dtoEncryptedFile).ConfigureAwait(false);
            var dtoKeyStoreDto = ComputeFile.GenererDtoKeyStore(cleDeChiffrement, encUrlResponse.Id);
            await _unitOfWork.KeyStoreRepository.Add(dtoKeyStoreDto).ConfigureAwait(false);
        }

        private static RequestResponse VerifierLaTailleDuFichier(IFormFile file)
        {
            const long maxSizeBytes = 5 * 1024 * 1024; // 5 Mo en octets

            if (file.Length > maxSizeBytes)
            {
                return new RequestResponse((int)HttpStatusCode.BadRequest, false, "Le fichier dépasse la taille maximale autorisée de 5 Mo", null);
            }

            return new RequestResponse();
        }

        #endregion
    }
}
