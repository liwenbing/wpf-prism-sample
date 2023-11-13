using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Component.Communication.ViewModels
{
    public class ModbusViewModel
    {
        private ModbusHelper _modbusHelper;
        public ModbusViewModel(ModbusHelper modbusHelper) 
        {
            _modbusHelper = modbusHelper;
        }
    }
}
