using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Text.RegularExpressions;

using GWF_WebServices.Entities.UserEntities;

namespace GWF_WebServices.Models
{
    public partial class GWF_User
    {
        private UserState default_state = new OfflineState();

        public UserState currentState
        {
            get
            {
                return this.default_state;
            }
            set
            {
                this.default_state = value;
            }
        }
    }
}