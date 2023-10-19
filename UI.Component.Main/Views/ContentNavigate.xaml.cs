using Core;
using Core.MenuNavigate;
using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.Component.Main.Views
{
    /// <summary>
    /// Dashboard.xaml 的交互逻辑
    /// </summary>
    public partial class ContentNavigate : UserControl
    {
        private readonly IRegionManager _regionManager;
        private readonly IApplicationCommands _applicationCommands;

        public ContentNavigate(IRegionManager regionManager, IApplicationCommands applicationCommands)
        {
            InitializeComponent();

            _regionManager = regionManager;

            RegionManager.SetRegionName(HamburgerMenuContent, Core.RegionNames.SubContentRegion);
            RegionManager.SetRegionManager(HamburgerMenuContent, regionManager);
            _applicationCommands = applicationCommands;
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {
            //_regionManager.RequestNavigate("SubContentRegion", "DashboardView");
            if (((HamburgerMenu)sender).SelectedItem is HamburgerMenuItem child)
            {
                if (child.CommandParameter is string path)
                {
                    _applicationCommands.NavigateCommand.Execute(path);
                }
            }
        }

        private void HamburgerMenuControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (((HamburgerMenu)sender).SelectedItem is IMenuRootItem root)
            {
                _applicationCommands.NavigateCommand.Execute(root.DefaultNavigationPath);
            }
        }
    }
}
