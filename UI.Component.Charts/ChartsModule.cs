using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace UI.Component.Charts
{
    public class ChartsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.ChartsRegion, "ChartsDemo");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.ChartsDemo, ViewModels.ChartsDemoViewModel>();
        }
    }
}
