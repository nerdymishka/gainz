namespace Nexus.Data
{
    public class PublicKeyRecord
    {
        public int Id { get; set; }

        public string UriPath  {get; set ;}

        public byte[] Blob  { get; set; }

        public int? UserId  { get; set; }

        public UserRecord User { get; set; }
    }
}