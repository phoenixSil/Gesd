using System;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Gesd.Api.Dtos;
using Gesd.Api.Repositories.Contrats;
using Gesd.Api.Settings;

using Fichier = Gesd.Entite.File;

namespace Gesd.Api.Repositories
{
    public class BlobRepository : IBlobRepository
    {
        private BlobServiceClient BlobServiceClient { get; }
        private BlobContainerClient BlobContainerClient { get; }

        private readonly FileSettings _fichierSetting;

        public BlobRepository(FileSettings fichierSetting)
        {
            _fichierSetting = fichierSetting;
            BlobServiceClient = new BlobServiceClient(_fichierSetting.ChaineDeConnectionBlob);
            BlobContainerClient = BlobServiceClient.GetBlobContainerClient(_fichierSetting.BlobContainerName);
        }

        public async Task<(string fileName, string filePath, int version)> Add(IFormFile file)
        {

            if (_fichierSetting.Environment == ToolsNeeds.LOCAL)
            {
                return await saveFileInLocalStorage(file).ConfigureAwait(false);
            }

            else
            {
                return await SaveFileInBlobStorage(file).ConfigureAwait(false);
            }
        }

        public async Task<bool> Delete(string filePath)
        {
            try
            {
                if (_fichierSetting.Environment != ToolsNeeds.LOCAL)
                {
                    return await SupprimerUnFichierDansLeBlob(filePath);
                }
                else
                {
                    return SupprimerUnFichierDansLeDossierLocal(filePath);
                }
            }
            catch (Exception ex)
            {
                // Gérez les exceptions ici
                Console.WriteLine($"Une erreur s'est produite lors de la suppression du blob : {ex.Message}");
                return false;
            }
        }


        #region PRIVATE FUNCTION

        private async Task<(string fileName, string filePath, int version)> SaveFileInBlobStorage(IFormFile file)
        {
            int version = 1;
            string fileName = Path.GetFileName(file.FileName);
            var blobClient = BlobContainerClient.GetBlobClient(fileName);

            if (await blobClient.ExistsAsync())
            {
                // Compter les fichiers qui ont le même nom dans le blob
                version = await CountFilesWithSameNameAsync(fileName);

                if (await IsFileIdenticalAsync(blobClient, file))
                {
                    return (fileName, blobClient.Uri.ToString(), version);
                }
                else
                {
                    // Créer un nouveau nom de fichier avec la version
                    blobClient = BlobContainerClient.GetBlobClient(file.FileName);

                    // Upload du nouveau fichier dans le blob
                    await using (var fileStream = file.OpenReadStream())
                    {
                        await blobClient.UploadAsync(fileStream, true);
                    }

                    return (file.FileName, blobClient.Uri.ToString(), version++);
                }
            }

            // Si le fichier n'existe pas dans le blob, l'uploader directement
            await using (var fileStream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(fileStream, true);
            }

            return (fileName, blobClient.Uri.ToString(), version);
        }

        private async Task<int> CountFilesWithSameNameAsync(string fileName)
        {
            int count = 1;
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = Path.GetExtension(fileName);

            string blobNameToCheck = $"{fileNameWithoutExtension}_{count}{fileExtension}";
            var blobClient = BlobContainerClient.GetBlobClient(blobNameToCheck);

            while (await blobClient.ExistsAsync())
            {
                count++;
                blobNameToCheck = $"{fileNameWithoutExtension}_{count}{fileExtension}";
                blobClient = BlobContainerClient.GetBlobClient(blobNameToCheck);
            }

            return count;
        }

        private async Task<bool> IsFileIdenticalAsync(BlobClient blobClient, IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(memoryStream);
                memoryStream.Position = 0;

                using var newMemoryStream = new MemoryStream();
                await file.CopyToAsync(newMemoryStream);
                newMemoryStream.Position = 0;

                return memoryStream.Length == newMemoryStream.Length &&
                       memoryStream.ToArray().SequenceEqual(newMemoryStream.ToArray());
            }
        }

        public async Task<FileAddedDto> AjouterAvecMetadonnees(FileToAddDto fileDto)
        {
            var nomFichier = Path.GetFileName(fileDto.File.FileName);          
            try
            {
                var existeFichier = await VerifierSiFichierExisteDansBlobAvecMetadonnees(fileDto).ConfigureAwait(false);
                if (existeFichier != null)
                {
                    return existeFichier;
                }

                const int version = 1;
                return await TeleverserFichierDansBlob(fileDto, version).ConfigureAwait(false);
            }
            catch (Exception)
            {
                var blobExistant = await BlobContainerClient.GetBlobClient(nomFichier).ExistsAsync();
                if (blobExistant)
                {
                    await BlobContainerClient.GetBlobClient(nomFichier).DeleteIfExistsAsync();
                }
                return null;
            }
        }

        private async Task<FileAddedDto> VerifierSiFichierExisteDansBlobAvecMetadonnees(FileToAddDto fileDto)
        {
            var blobs = BlobContainerClient.GetBlobs();
            foreach (var blobItem in blobs)
            {
                var blobClient = BlobContainerClient.GetBlobClient(blobItem.Name);
                var metadata = await blobClient.GetPropertiesAsync();

                var titreFichierExistant = metadata.Value.Metadata["FileTitle"];
                var descriptionExistante = metadata.Value.Metadata["Description"];
                var tailleFichierExistante = double.Parse(metadata.Value.Metadata["FileSize"]);
                var versionFichierExistante = int.Parse(metadata.Value.Metadata["Version"]);

                if (titreFichierExistant == fileDto.FileTitle &&
                    descriptionExistante == fileDto.Description &&
                    tailleFichierExistante == fileDto.FileSize)
                {
                    return new FileAddedDto
                    {
                        FileTitle = titreFichierExistant,
                        Description = descriptionExistante,
                        IdDocument = Int16.Parse(metadata.Value.Metadata["IdDocument"]),
                        Categorie = metadata.Value.Metadata["Categorie"],
                        FileType = metadata.Value.Metadata["FileType"],
                        Url = blobClient.Uri.ToString()
                    };
                }
                if(titreFichierExistant == Path.GetFileNameWithoutExtension(fileDto.File.FileName) &&
                    tailleFichierExistante != fileDto.FileSize)
                {
                    var version = versionFichierExistante++;
                    return await TeleverserFichierDansBlob(fileDto, version).ConfigureAwait(false);
                }
            }
            return null;
        }

        private async Task<FileAddedDto> TeleverserFichierDansBlob(FileToAddDto fileDto, int version)
        {
            var nomFichier = Path.GetFileName(fileDto.File.FileName);
            var extensionFichier = Path.GetExtension(nomFichier);
            var fileName = Path.GetFileNameWithoutExtension(nomFichier);

            var blobClient = BlobContainerClient.GetBlobClient(nomFichier);
            await blobClient.UploadAsync(fileDto.File.OpenReadStream(), true);

            var metadata = new Dictionary<string, string>
            {
                { "FileTitle", fileName },
                { "Description", "fileDto.Description" },
                { "IdDocument", fileDto.IdDocument.ToString() },
                { "FileType", fileDto.FileType },
                { "Categorie", fileDto.Categorie },
                { "FileSize", fileDto.FileSize.ToString() },
                { "FileExtension", extensionFichier },
                { "Version", version.ToString() },
                { "CreatedDate", DateTime.UtcNow.ToString() },
                { "LastModify", DateTime.UtcNow.ToString() },
            };

            await blobClient.SetMetadataAsync(metadata);

            var blobUri = blobClient.Uri.ToString();

            return new FileAddedDto
            {
                FileTitle = fileDto.FileTitle,
                Description = fileDto.Description,
                IdDocument = fileDto.IdDocument,
                Categorie = fileDto.Categorie,
                FileType = fileDto.FileType,
                Url = blobUri
            };
        }



        private async Task<(string fileName, string filePath, int version)> saveFileInLocalStorage(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_fichierSetting.CheminLocalFichier, _fichierSetting.FolderName);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            for (int suffix = 1; File.Exists(filePath); suffix++)
            {
                fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{suffix}{Path.GetExtension(file.FileName)}";
                filePath = Path.Combine(uploadsFolder, fileName);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return (fileName, filePath, 1);
        }

        private bool SupprimerUnFichierDansLeDossierLocal(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        private async Task<bool> SupprimerUnFichierDansLeBlob(string filePath)
        {
            // Obtenez une référence à un blob existant
            var blobClient = BlobContainerClient.GetBlobClient(_fichierSetting.BlobContainerName);

            // Supprimez le blob
            await blobClient.DeleteIfExistsAsync();
            return true;
        }

        public Task<Entite.File> Get()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Fichier>> GetAllMetadata()
        {
            var blobFilesWithMetadata = new List<(BlobItem, IDictionary<string, string>, string)>();

            await foreach (var blobItem in BlobContainerClient.GetBlobsAsync())
            {
                var blobClient = BlobContainerClient.GetBlobClient(blobItem.Name);
                var blobProperties = await blobClient.GetPropertiesAsync();

                var metadata = blobProperties.Value.Metadata;
                blobFilesWithMetadata.Add((blobItem, metadata, blobClient.Uri.ToString()));
            }

            var fichiers = new List<Fichier>();
            foreach (var (_, metadata, url) in blobFilesWithMetadata)
            {
                var fichier = new Fichier
                {
                    Id = Guid.NewGuid(),
                    FileTitle = metadata.ContainsKey("FileTitle") ? metadata["FileTitle"] : null,
                    Description = metadata.ContainsKey("Description") ? metadata["Description"] : null,
                    CreatedDate = metadata.ContainsKey("CreatedDate") ? DateTime.Parse(metadata["CreatedDate"]) : DateTime.MinValue,
                    LastModify = metadata.ContainsKey("LastModify") ? DateTime.Parse(metadata["LastModify"]) : DateTime.MinValue,
                    IdDocument = metadata.ContainsKey("IdDocument") ? int.Parse(metadata["IdDocument"]) : 0,
                    FileSize = metadata.ContainsKey("FileSize") ? double.Parse(metadata["FileSize"]) : 0,
                    FileType = metadata.ContainsKey("FileType") ? metadata["FileType"] : null,
                    Version = metadata.ContainsKey("Version") ? int.Parse(metadata["Version"]) : 0,
                    Categorie = metadata.ContainsKey("Categorie") ? metadata["Categorie"] : null,
                    FileExtension = metadata.ContainsKey("FileExtension") ? metadata["FileExtension"] : null,
                    Url = url
                };

                fichiers.Add(fichier);
            }

            return fichiers;
        }

        public async Task<string> TelechargerFichier(string url)
        {
            // Génère un nom de fichier temporaire pour stocker le contenu téléchargé
            var destinationFilePath = Path.GetTempFileName();

            // Parse le nom du conteneur et le nom du blob à partir de l'URL du blob
            var blobUri = new Uri(url);
            var blobName = string.Concat(blobUri.Segments[2..]);

            // Initialise le client BlobServiceClient avec la chaîne de connexion
            var blobClient = BlobContainerClient.GetBlobClient(blobName);

            // Télécharge le contenu du blob dans le fichier spécifié
            await blobClient.DownloadToAsync(destinationFilePath);

            return destinationFilePath;
        }


        #endregion PRIVATE FUNCTION
    }
}
