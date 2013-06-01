using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GWF_WebServices.Types;

namespace GWF_WebServices.Utils.MessageUtils
{
    public interface IMessageHandler: IHandler
    {
        void setMessage(GWFMessage message);
    }
}
