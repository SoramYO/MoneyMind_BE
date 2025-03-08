using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_DAL.Entities
{

public class UserFcmToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string FcmToken { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsed { get; set; }
} }