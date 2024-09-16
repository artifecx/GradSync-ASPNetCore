using Data.Models;
using Services.ServiceModels;
using System.Threading.Tasks;
using static Resources.Constants.Enums;

namespace Services.Interfaces
{
    public interface IEducationService
    {
        Task<EducationViewModel> GetAllAsync();
    }
}
