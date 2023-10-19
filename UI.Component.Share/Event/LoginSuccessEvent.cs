using Core.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Component.Share.Event
{
    public class LoginSuccessEvent:PubSubEvent<AccountDto>
    {
    }
}
