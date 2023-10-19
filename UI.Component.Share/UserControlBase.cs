using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.Component.Share
{
    public partial class UserControlBase:UserControl
    {
        public UserControlBase() 
        {
            this.Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }
    }
}
