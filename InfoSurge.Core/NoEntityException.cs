using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSurge.Core
{
    public class NoEntityException : Exception
    {
        public NoEntityException(string message)
            : base(message)
        {
        }
    }
}
