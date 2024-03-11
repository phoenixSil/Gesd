using System.ComponentModel.DataAnnotations;

namespace Gesd.Entite
{
    public class EncryptedUrlFile
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string EncryptedUrl { get; set; }

        public Guid FileId { get; set; }
        public File File { get; set; }

        public KeyStore GenerateStoredKey { get; set; }
    }
}
