using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIBusinessLayer.Auth.DTOs
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
        public string UserName { get; set; }
    }
}
