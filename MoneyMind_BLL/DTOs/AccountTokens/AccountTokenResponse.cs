﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs.AccountTokens
{
    public class AccountTokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
