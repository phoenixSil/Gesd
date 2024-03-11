using AutoMapper;

using Gesd.Api.Dtos;
using Gesd.Api.Dtos.Validators;
using Gesd.Api.Features.Commands;
using Gesd.Api.Features.Communs;
using Gesd.Api.Features.Tools;
using Gesd.Api.Repositories.Contrats;
using Gesd.Api.Settings;
using Gesd.Entite.Responses;
using MediatR;
using System.Net;

using File = Gesd.Entite.File;

namespace Gesd.Api.Features.Handlers
{
    public class AddFileWithoutDatabaseCmdHandler : BaseComputeCmdHandler<AddFileWithoutDatabaseCmd>
    {
        public AddFileWithoutDatabaseCmdHandler(IMediator mediator, IUnitOfWork unitOfWork, IMapper mapper, ILogger<AddFileWithoutDatabaseCmdHandler> logger)
            : base(mediator, unitOfWork, mapper, logger)
        {
        }

        public async override Task<RequestResponse> Handle(AddFileWithoutDatabaseCmd request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Vérification de la Taille du fichier");
            VerifierLaTailleDuFichier(request.FileDto.File);

            _logger.LogInformation("Début de Traitement");

            var validator = new AddFileValidator();
            var dto = request.FileDto;
            dto.FileSize = ComputeFile.GetFileSizeInMegabytes(dto.File);
            var result = await validator.ValidateAsync(dto, cancellationToken).ConfigureAwait(false);

            if (!result.IsValid)
            {
                return ApiResponse<File>.CreateErrorResponse(HttpStatusCode.InternalServerError, "Une erreur est survenue dans le serveur", result.Errors.Select(q => q.ErrorMessage).ToList());
            }

            string? url = null;

            try
            {
                _logger.LogInformation("Ajout du fichier dans le Blob");
                var response = await _unitOfWork.BlobRepository.AjouterAvecMetadonnees(request.FileDto).ConfigureAwait(false);
                url = response.Url;

                 return ApiResponse<File>.CreateSuccessResponse(_mapper.Map<File>(response), "File added successfully");
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