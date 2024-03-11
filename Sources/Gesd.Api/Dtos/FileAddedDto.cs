namespace Gesd.Api.Dtos
{
    public class FileAddedDto : IFileDto
    {
        public string FileTitle { get; set; }
        public string Description { get; set; }
        public int IdDocument { get; set; }
        public string FileType { get; set; }
        public string Categorie { get; set; }
        public string Url { get; internal set; }
    }
}