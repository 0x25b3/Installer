#region License
// Copyright (C) 2018 Benjamin Bartels
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of  MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// this program.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Installer.Controls
{
    public partial class EditableLabel : UserControl, INotifyPropertyChanged
    {
        private bool isEditEnabled = false;

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableLabel), new FrameworkPropertyMetadata("EditableLabel", OnTextChanged));
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty ShowVerticalScrollbarProperty = DependencyProperty.Register("ShowVerticalScrollbar", typeof(bool), typeof(EditableLabel), new FrameworkPropertyMetadata(false));
        public bool ShowVerticalScrollbar
        {
            get { return (bool)GetValue(ShowVerticalScrollbarProperty); }
            set { SetValue(ShowVerticalScrollbarProperty, value); }
        }

        public bool IsEditEnabled
        {
            get { return isEditEnabled; }
            set { SetProperty(ref isEditEnabled, value); }
        }

        public EditableLabel()
        {
            InitializeComponent();
            
        }

        private void ContentBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IsEditEnabled = !IsEditEnabled;
            if (IsEditEnabled)
            {
                ContentBox.Focus();
                ContentBox.SelectionStart = ContentBox.Text.Length;
                ContentBox.SelectionLength = 0;
            }
            else
                Keyboard.ClearFocus();
        }
        private void ContentBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditEnabled = false;
        }

        private static void OnTextChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var Source = source as EditableLabel;
            
        }
        
        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged([CallerMemberName] string PropertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        internal void SetProperty<T>(ref T Variable, T Value, [CallerMemberName] string PropertyName = "")
        {
            Variable = Value;
            NotifyPropertyChanged(PropertyName);
        }
    }
}
