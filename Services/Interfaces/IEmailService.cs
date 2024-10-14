using Data.Models;
using Services.ServiceModels;
using System.Threading.Tasks;
using static Resources.Constants.Enums;

namespace Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
