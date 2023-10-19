using Core;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Component.Index.ViewModels;

namespace UI.Component.Index
{
    public class DashboardModule:IModule
    {
        private readonly IRegionManager _regionManager;

        public DashboardModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MenuRegion, typeof(Views.DashboardMenu));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Dashboard1, Dashboard1>();
            containerRegistry.RegisterForNavigation<Views.Dashboard2, Dashboard2>();
        }
    }
}
