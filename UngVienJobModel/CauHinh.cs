using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace UngVienJobModel
{
    public class CauHinh
    {
        [Keyword]
        public string username { get; set; }
        [Keyword]
        public string password { get; set; }
        [Keyword]
        public string url_login { get; set; }
        [Keyword]
        public string xpath_username { get; set; }
        [Keyword]
        public string xpath_password { get; set; }
    }
}
