using Gesd.Entite;
using System.ComponentModel.DataAnnotations;

namespace Gesd.Api.Dtos
{
    public interface IFileDto
    {
        public string FileTitle { get; set; }
        public string Description { get; set; }
        public int IdDocument { get; set; }
        public string FileType { get; set; }
        public string Categorie { get; set; }
    }
}
