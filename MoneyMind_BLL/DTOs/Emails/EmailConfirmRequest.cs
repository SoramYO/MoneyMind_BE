﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.Emails
{
    public class EmailConfirmRequest
    {
        public string Username { get; set; }
        public string ConfirmationLink { get; set; }
    }
}
