namespace Edu_Block_dev.Modal.Dock
{
	public class IssuerCredentialRequest
	{
        public string RecipientEmail { get; set; }
        public string Email { get; set; }
        public string RollNo { get; set; }
        public Guid userProfileID { get; set; }
        public string Issuer { get; set; }
        public string Template { get; set; }
        public string Schema { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string DegreeName { get; set; }
        public string DegreeType { get; set; }
        public string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime IssuanceDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}