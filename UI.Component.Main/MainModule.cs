using Core;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Component.Main
{
    [Module(ModuleName = ModuleNames.MainModule, OnDemand = true)]
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.TopMenuRegion, typeof(Views.TopMenu));
            //regionManager.RegisterViewWithRegion("ContentRegion", typeof(Views.Main));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Main, ViewModules.MainViewModel>(ViewNames.MainView);
            containerRegistry.RegisterForNavigation<Views.TopMenu, ViewModules.TopMenuViewModel>(ViewNames.TopMenuView);
            containerRegistry.RegisterForNavigation<Views.ContentNavigate, ViewModules.ContentNavigateViewModel>(ViewNames.ContentNavigateView);

        }
    }
}
