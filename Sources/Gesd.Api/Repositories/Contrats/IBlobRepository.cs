using System;

using Gesd.Api.Dtos;

using File = Gesd.Entite.File;

namespace Gesd.Api.Repositories.Contrats
{
    public interface IBlobRepository
    {
        Task<(string fileName, string filePath, int version)> Add(IFormFile file);
        Task<FileAddedDto> AjouterAvecMetadonnees(FileToAddDto fileDto);
        Task<bool> Delete(string url);
        Task<File> Get();
        Task<List<File>> GetAllMetadata();
    }
}
