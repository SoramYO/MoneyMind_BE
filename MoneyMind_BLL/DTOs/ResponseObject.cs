﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMind_BLL.DTOs
{
    public class ResponseObject
    {
        public HttpStatusCode Status { get; set; }
        public String Message { get; set; }
        public Object Data { get; set; }
    }
}
