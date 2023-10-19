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
using UI.Conponent.FaceDetection.ViewModels;

namespace UI.Conponent.FaceDetection.Views
{
    /// <summary>
    /// FaceDetection.xaml 的交互逻辑
    /// </summary>
    public partial class FaceDetectionView : UserControl
    {
        public FaceDetectionView()
        {
            InitializeComponent();

            //this.Loaded += FaceDetectionView_Loaded;

        }

        private void FaceDetectionView_Loaded(object sender, RoutedEventArgs e)
        {
            videoView.Source = (DataContext as FaceDetectionViewModel).writeableBitmap;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var vm = (DataContext as FaceDetectionViewModel);
            //vm.SelectionChanged();
            //videoView.Source = vm.writeableBitmap;
        }
    }
}
