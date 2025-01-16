
using MoneyMind_BLL.DTOs.Emails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmEmailAsync(string toEmail, string subject, string templatePath, EmailConfirmRequest emailConfirmRequest);
    }
}
