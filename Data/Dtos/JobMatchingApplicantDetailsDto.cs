using System.Collections.Generic;

namespace Data.Dtos;

public partial class JobMatchingApplicantDetailsDto
{
    public string applicant_id { get; set; }
    public string resume_text { get; set; }
    public List<string> technical_skills { get; set; }
    public List<string> cultural_skills { get; set; }
    public string department_id { get; set; }
}
