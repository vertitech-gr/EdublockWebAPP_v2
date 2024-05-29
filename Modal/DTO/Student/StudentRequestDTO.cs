using Newtonsoft.Json;

namespace EduBlock.Model.DTO;

public class StudentRequestDTO
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class Attribute
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string MappedKey { get; set; }
    public bool Required { get; set; }
    public string Value { get; set; }
}

public class MappedArrayDTO
{
    public string Name { get; set; }
    public List<Attribute> Attributes { get; set; }
}

public class SchemaAttribute
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
    public bool Required { get; set; }
}


