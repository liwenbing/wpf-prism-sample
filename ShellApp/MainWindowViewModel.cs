using Core;
using Core.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using UI.Component.Share.Event;

namespace ShellApp
{
    public class MainWindowViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IModuleManager _moduleManager;

        public ICommand LoadedCommand {  get; }

        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IModuleManager moduleManager)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _moduleManager = moduleManager;

            _eventAggregator.GetEvent<LoginSuccessEvent>().Subscribe(LoginSuccess);

            LoadedCommand = new DelegateCommand(Loaded);
        }

        private void Loaded()
        {
            // TODO
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.LoginView);
        }

        private void LoginSuccess(AccountDto accountDto)
        {
            _moduleManager.LoadModule(ModuleNames.MainModule);
            // TODO
            _regionManager.RequestNavigate(RegionNames.MainRegion, ViewNames.MainView);
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }


    }
}
