using Core;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Conponent.FaceDetection
{
    [Module(ModuleName = ModuleNames.FaceDetectionModule, OnDemand = true)]
    public class FaceDetectionModule : Prism.Modularity.IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Views.FaceDetectionView, ViewModels.FaceDetectionViewModel>();

        }
    }
}
