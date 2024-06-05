using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Service.Exceptions
{
    public class DublicateEntityException:Exception
    {
        public DublicateEntityException(string msg):base(msg) { }
        public DublicateEntityException()
        {
            
        }
    }
}
