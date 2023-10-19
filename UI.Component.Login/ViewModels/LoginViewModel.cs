using Core;
using Core.Interface;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using UI.Component.Share.Event;

namespace UI.Component.Login.ViewModels
{
    public class LoginViewModel
    {
        private readonly IContainerExtension _container;
        private readonly IRegionManager _regionManager;
        private readonly IModuleManager _moduleManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IAccountService _accountService;

        public LoginViewModel(IContainerExtension container,IRegionManager regionManager,IModuleManager moduleManager, IEventAggregator eventAggregator, IAccountService accountService)
        {
            LoginCommand = new DelegateCommand(Login);
            _regionManager = regionManager;
            _container = container;
            _moduleManager = moduleManager;
            _eventAggregator = eventAggregator;
            _accountService = accountService;
        }

        public ICommand LoginCommand { get; set; }

        public string UserName { get; set; } = "admin";

        public string Password { get; set; } = "123456";

        private void Login()
        {
            if (_accountService.Login(UserName, Password))
            {
                _regionManager.RequestNavigate(RegionNames.MainRegion, "ChartsDemo");

                _eventAggregator.GetEvent<LoginSuccessEvent>().Publish(new Core.Models.AccountDto() { UserName = "admin" });
                //_moduleManager.LoadModule("ChartsModule");
                //_regionManager.RequestNavigate(RegionNames.ChartsRegion, "ChartsDemo");
            }
            else
            {
                MessageBox.Show("用户名或密码错误");
            }
        }

    }
}
