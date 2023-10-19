using Core;
using Microsoft.Xaml.Behaviors.Core;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UI.Component.Main.ViewModules
{
    public class MainViewModel
    {
        private readonly IRegionManager _regionManager;

        public ICommand LoadedCommand { get; }
        public ICommand NavigateCommand { get; }


        public MainViewModel(IRegionManager regionManager) 
        {
            _regionManager = regionManager;

            LoadedCommand = new DelegateCommand(Loaded);
            NavigateCommand = new ActionCommand(Navigate);

        }

        private void Navigate(object viewName)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName.ToString());
        }

        private void Loaded()
        {
            // TODO
            _regionManager.RequestNavigate(RegionNames.ContentRegion, ViewNames.ContentNavigateView);
        }

    }

    
}
