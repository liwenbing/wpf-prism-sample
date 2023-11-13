using Core;
using Core.Interface;
using Core.Services;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Component.Communication
{
    public class CommunicationModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.CommunicationRegion, "CommunicationDemo");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.ModbusView, ViewModels.ModbusViewModel>();

            containerRegistry.RegisterSingleton<ModbusHelper>();
        }
    }
}
