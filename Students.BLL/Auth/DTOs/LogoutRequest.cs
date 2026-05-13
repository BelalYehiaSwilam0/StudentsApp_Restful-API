using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIBusinessLayer.Auth.DTOs
{
    public class LogoutRequest
    {
        public string UserName { get; set; }
        public string RefreshToken { get; set; }
    }
}
