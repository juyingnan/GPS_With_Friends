using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GWF_Services.Utils
{
    public interface IMessageManager <ID_Type, T>
    {
        /**
         * Remove a meesage from the message manager
         */
        T getMessage ();

        /**
         * Add message to this Manager as the last one
         */
        void addMessage (T message);

        /**
         * Put message into the specified position
         */
        void putMessage (ID_Type id, T message); 
    }
}