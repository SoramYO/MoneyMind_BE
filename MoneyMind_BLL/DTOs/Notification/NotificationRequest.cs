
using MoneyMind_BLL.DTOs.GoalItems;
using MoneyMind_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Notification
{
    public class NotificationRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string FcmToken { get; set; }
    public Dictionary<string, string> Data { get; set; }
    }
} 