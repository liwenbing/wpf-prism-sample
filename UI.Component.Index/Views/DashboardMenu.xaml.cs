using Core.MenuNavigate;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.Component.Index.Views
{
    /// <summary>
    /// DashboardMenu.xaml 的交互逻辑
    /// </summary>
    public partial class DashboardMenu : HamburgerMenuItemCollection, IMenuRootItem
    {
        public DashboardMenu()
        {
            InitializeComponent();
        }

        public string DefaultNavigationPath => nameof(Dashboard1);
    }
}
