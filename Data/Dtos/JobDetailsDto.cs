using System.Collections.Generic;

namespace Data.Dtos;

public partial class JobDetailsDto
{
    public string job_id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public List<string> technical_skills { get; set; }
    public List<string> cultural_skills { get; set; }
    public decimal? tech_cult_weight { get; set; }
    public List<string> department_ids { get; set; }
}
