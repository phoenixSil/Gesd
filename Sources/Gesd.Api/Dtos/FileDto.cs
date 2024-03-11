using Gesd.Entite;
using System.ComponentModel.DataAnnotations;

namespace Gesd.Api.Dtos
{
    public class FileDto : IFileDto
    {
        public Guid Id { get; set; }
        public string FileTitle { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModify { get; set; }
        public int IdDocument { get; set; }
        public double FileSize { get; set; }
        public string FileType { get; set; }
        public int Version { get; set; }
        public string Categorie { get; set; }
        public string FileExtension { get; set; }
        public string Url { get; set; }
    }
}
