using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIBusinessLayer.Auth.DTOs
{
    public class TokenResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
