namespace Edu_Block_dev.Modal.DTO
{
    public class EnvelopeDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public List<Certificate> Credentials { get; set; }
    }

    public class EnvelopeDTOC
    {
        public string Name { get; set; }
        public List<Guid> Credentials { get; set; }
    }

    public class EnvelopeCreationDto
    {
        public Guid EnvelopID { get; set; }
        public string Email { get; set; }
        public List<Guid> Credentials { get; set; }
    }

    public class EnvelopeUrlDto
    {
        public Guid EnvelopID { get; set; }
    }
}
