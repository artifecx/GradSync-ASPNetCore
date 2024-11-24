using System.Collections.Generic;

namespace Data.Dtos;

public partial class ApplicantDto
{
    public string ApplicantId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<string> TechnicalSkills { get; set; }
    public List<string> CulturalSkills { get; set; }
    public List<string> Certifications { get; set; }
    public string ProgramName { get; set; }
    public string DepartmentName { get; set; }
    public string CollegeName { get; set; }
    public string YearLevelName { get; set; }
}
