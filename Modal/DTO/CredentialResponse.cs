namespace Edu_Block_dev.Modal.DTO
{
   using System;
using System.Collections.Generic;

public class Context
{
    public List<string> W3org2018CredentialsV1 { get; set; }
    public Dictionary<string, string> Dk { get; set; }
    public string Aditya { get; set; }
    public string Name { get; set; }
}

public class CredentialSubject
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class CredentialSchema
{
    public string Id { get; set; }
    public string Type { get; set; }
}

public class PrettyVC
{
    public string Type { get; set; }
    public string Orientation { get; set; }
    public string Size { get; set; }
    public string Proof { get; set; }
}

public class Proof
{
    public string Type { get; set; }
    public DateTime Created { get; set; }
    public string VerificationMethod { get; set; }
    public string ProofPurpose { get; set; }
    public string Jws { get; set; }
}

public class CredentialResponse
    {
    public List<object> Context { get; set; }
    public string Id { get; set; }
    public List<string> Type { get; set; }
    public CredentialSubject CredentialSubject { get; set; }
    public DateTime IssuanceDate { get; set; }
    public string Issuer { get; set; }
    public CredentialSchema CredentialSchema { get; set; }
    public string Name { get; set; }
    public PrettyVC PrettyVC { get; set; }
    public Proof Proof { get; set; }
}


}
