using System.ComponentModel.DataAnnotations;

using Gesd.Entite;

namespace Gesd.Api.Dtos
{
    public class FileToAddDto : IFileDto
    {
        public string FileTitle { get; set; }
        public string Description { get; set; }
        public int IdDocument { get; set; }
        [Required]
        public string FileType { get; set; }
        [Required]
        public string Categorie { get; set; }
        public double FileSize { get; set; }
        public string FileExtension { get; set; }
        public int Version { get; set; }
        public IFormFile File { get; set; }
    }
}
