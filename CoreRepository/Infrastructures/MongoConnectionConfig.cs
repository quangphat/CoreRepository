using System;
using System.Collections.Generic;
using System.Text;

namespace CoreRepository.Infrastructures
{
    public class MongoConnectionConfig
    {
        public string ServerURl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}
