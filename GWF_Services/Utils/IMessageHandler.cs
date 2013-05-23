using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GWF_Services.Types;

namespace GWF_Services.Utils
{
    public interface IMessageHandler: IHandler
    {
        void setMessage(GWFMessage message);
    }
}
