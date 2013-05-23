using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GWF_Services
{
    public interface IMessageHandler: IHandler
    {
        public void setMessage();
    }
}
