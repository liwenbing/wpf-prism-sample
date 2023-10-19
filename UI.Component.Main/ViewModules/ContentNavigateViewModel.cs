using Core.MenuNavigate;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Component.Main.ViewModules
{
    public class ContentNavigateViewModel
    {
        private readonly IRegionManager _regionManager;

        private DelegateCommand<string> _navigateCommand;
        public DelegateCommand<string> NavigateCommand => _navigateCommand ?? (_navigateCommand = new DelegateCommand<string>(ExecuteNavigateCommand));

        public ContentNavigateViewModel(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            _regionManager = regionManager;
            applicationCommands.NavigateCommand.RegisterCommand(NavigateCommand);
        }

        private void ExecuteNavigateCommand(string navigationPath)
        {
            if (string.IsNullOrEmpty(navigationPath))
            {
                throw new ArgumentNullException(nameof(navigationPath));
            }
            _regionManager.RequestNavigate(Core.RegionNames.SubContentRegion, navigationPath);
        }

    }
}
