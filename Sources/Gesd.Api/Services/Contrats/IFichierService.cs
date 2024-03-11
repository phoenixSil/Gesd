using Gesd.Api.Dtos;
using Gesd.Entite.Responses;

namespace Gesd.Api.Services.Contrats
{
    public interface IFichierService
    {
        Task<ApiResponse<FileAddedDto>?> Add(string categorie, string type, IFormFile file);
        Task<ApiResponse<FileAddedDto>?> AddWithoutDataBase(string category, string type, IFormFile file);
        Task<ApiResponse<bool>> Delete(Guid id);
        Task<ApiResponse<List<FileDto>>> Get();
    }
}
