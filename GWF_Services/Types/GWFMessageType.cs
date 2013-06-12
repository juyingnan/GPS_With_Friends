using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GWF_WebServices.Types
{
    public enum GWFMessageType
    {
        DEFAULT_TYPE = 0,
        IM_MESSAGE = 1,
        REQUEST = 2,
        NOTIFICATION = 3
    }
}