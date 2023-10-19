using Core;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Component.Login
{
    [Module(ModuleName = ModuleNames.LoginModule, OnDemand = false)]
    public class LoginModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.LoginRegion, typeof(Views.Login));
            
            //regionManager.RegisterViewWithRegion(RegionNames.LoginRegion, "Register");
            //regionManager.RequestNavigate(RegionNames.MainRegion, "Login");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.Login, ViewModels.LoginViewModel>();
            containerRegistry.RegisterForNavigation<Views.Register, ViewModels.RegisterViewModel>();
        }
    }
}
