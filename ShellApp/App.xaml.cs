using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using Core.Interface;
using Core.MenuNavigate;
using Core.Services;
using Core.UI.RegionAdapters;
using LiveCharts.Wpf;
using MahApps.Metro.Controls;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using UI.Component.Charts;
using UI.Component.Index;
using UI.Component.Login;
using UI.Component.Main;
using UI.Conponent.FaceDetection;

namespace ShellApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        { 
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // containerRegistry.Register<Services.ICustomerStore, Services.DbCustomerStore>();
            // register other needed services here

            // menu navigate command
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();

            containerRegistry.Register<IAccountService, AccountService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<LoginModule>();
            moduleCatalog.AddModule<MainModule>();
            moduleCatalog.AddModule<ChartsModule>();
            moduleCatalog.AddModule<FaceDetectionModule>();
            moduleCatalog.AddModule<DashboardModule>();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);

            // Custom Adapters
            regionAdapterMappings.RegisterMapping(typeof(HamburgerMenu), Container.Resolve<MahAppsHamburgerMenuRegionAdapter>());
        }
    }
}
