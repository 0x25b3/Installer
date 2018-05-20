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

using System.Collections.ObjectModel;
using Installer.Base;

namespace Installer
{
    public class InstallerViewModel : ViewModel
    {
        private ViewModel currentPage;
        private int currentIndex = 0;
        private bool backButtonVisible = false, cancelButtonVisible = true;

        public bool IsEditMode { get => InstallerWindow.IsEditMode; }
        public ObservableCollection<ViewModel> Pages { get; internal set; } = new ObservableCollection<ViewModel>();
        public ViewModel CurrentPage { get { return currentPage; } set { SetProperty(ref currentPage, value); } }
        public int CurrentIndex { get { return currentIndex; } set { SetProperty(ref currentIndex, value); } }

        public bool BackButtonVisible { get { return backButtonVisible; } set { SetProperty(ref backButtonVisible, value); } }
        public bool CancelButtonVisible { get { return cancelButtonVisible; } set { SetProperty(ref cancelButtonVisible, value); } }
    }
}