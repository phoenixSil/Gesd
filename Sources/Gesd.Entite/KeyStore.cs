namespace Gesd.Entite
{
    public class KeyStore
    {
        public Guid Id { get; set; }
        public string GeneratedKey { get; set; }
        public Guid EncryptedUrlId { get; set; }
        public EncryptedUrlFile EncryptedUrlFile { get; set; }
    }
}
