using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Core.UI
{
    public class PasswordBoxBindingHelper
    {
        public static readonly DependencyProperty PasswordProperty =
                               DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxBindingHelper), new PropertyMetadata(""));

        public static readonly DependencyProperty AttachProperty =
                               DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxBindingHelper), new FrameworkPropertyMetadata(OnAttachPropertyChanged));


        public static string GetPassword(DependencyObject obj)
        {
            return (string)obj.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject obj, string value)
        {
            obj.SetValue(PasswordProperty, value);
        }


        public static bool GetAttach(DependencyObject obj)
        {
            return (bool)obj.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject obj, bool value)
        {
            obj.SetValue(AttachProperty, value);
        }


        private static void OnAttachPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var passwordBox = obj as PasswordBox;
            if (passwordBox != null)
            {
                passwordBox.PasswordChanged -= PasswordBoxPasswordChanged;
                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordBoxPasswordChanged;
                }
            }
        }

        private static void PasswordBoxPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = (PasswordBox)sender;
            if (!string.Equals(GetPassword(passwordBox), passwordBox.Password))
            {
                SetPassword(passwordBox, passwordBox.Password);
            }
        }

    }
}
