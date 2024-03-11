using System.Net;

using AutoMapper;
using Gesd.Api.Dtos;
using Gesd.Api.Features.Commands;
using Gesd.Api.Services.Contrats;
using Gesd.Entite.Responses;
using MediatR;

using File = Gesd.Entite.File;

namespace Gesd.Api.Services
{
    public class FichierService : IFichierService
    {
        private readonly IMediator _sender;
        private readonly IMapper _mapper;

        public FichierService(IMediator sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        public async Task<ApiResponse<FileAddedDto>?> AddWithoutDataBase(string category, string type, IFormFile file)
        {
            try
            {
                // Validation des paramètres
                if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(type) || file == null)
                {
                    return ApiResponse<FileAddedDto>.CreateBadRequestResponse("Categorie, type, or file cannot be null or empty.");
                }

                var filedto = new FileToAddDto
                {
                    File = file,
                    Categorie = category,
                    FileType = type
                };

                var response = await _sender.Send(new AddFileWithoutDatabaseCmd { FileDto = filedto }).ConfigureAwait(false);

                // Gestion des exceptions
                if (!response.Success)
                {
                    // Gérer l'échec de la commande
                    return ApiResponse<FileAddedDto>.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to add file.", response.Errors);
                }

                // Mapper le résultat vers FileAddedDto
                var dtoFileAdded = _mapper.Map<FileAddedDto>(((ApiResponse<File>)response).Data);

                return ApiResponse<FileAddedDto>.CreateSuccessResponse(dtoFileAdded);
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
                return ApiResponse<FileAddedDto>.CreateErrorResponse(ex, ex.Message);
            }
        }

        public async Task<ApiResponse<FileAddedDto>?> Add(string categorie, string type, IFormFile file)
        {
            try
            {
                // Validation des paramètres
                if (string.IsNullOrEmpty(categorie) || string.IsNullOrEmpty(type) || file == null)
                {
                    return ApiResponse<FileAddedDto>.CreateBadRequestResponse("Categorie, type, or file cannot be null or empty.");
                }

                var filedto = new FileToAddDto
                {
                    File = file,
                    Categorie = categorie,
                    FileType = type
                };

                var response = await _sender.Send(new AddFileCmd { FileDto = filedto }).ConfigureAwait(false);

                // Gestion des exceptions
                if (!response.Success)
                {
                    // Gérer l'échec de la commande
                    return ApiResponse<FileAddedDto>.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to add file.", response.Errors);
                }

                // Mapper le résultat vers FileAddedDto
                var dtoFileAdded = _mapper.Map<FileAddedDto>(((ApiResponse<File>)response).Data);

                return ApiResponse<FileAddedDto>.CreateSuccessResponse(dtoFileAdded);
            }
            catch (Exception ex)
            {
                // Gérer les exceptions
                return ApiResponse<FileAddedDto>.CreateErrorResponse(ex, ex.Message);
            }
        }


        public async Task<ApiResponse<List<FileDto>>> Get()
        {
            var response = await _sender.Send(new GetFileCmd()).ConfigureAwait(false);
            var data = ((ApiResponse<IEnumerable<File>>)response).Data;
            var dto = _mapper.Map<List<FileDto>>(data);
            return new ApiResponse<List<FileDto>>
            {
                Data = dto,
                Errors = null,
                Message = response.Message,
                StatusCode = response.StatusCode,
                Success = response.Success,
            };
        }

        public Task<ApiResponse<bool>> Delete(Guid id)
        {
            throw new NotImplementedException();
        }
        

    }
}
