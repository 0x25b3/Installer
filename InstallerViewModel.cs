using Installer.Base;
using System.Collections.ObjectModel;

namespace Installer
{
    public class InstallerViewModel : ViewModel
    {
        private bool isEditMode;
        private ViewModel currentPage;
        private int currentIndex = 0;
        private bool backButtonVisible = false;

        public bool IsEditMode { get { return isEditMode; } set { SetProperty(ref isEditMode, value); } }
        public ObservableCollection<ViewModel> Pages { get; internal set; } = new ObservableCollection<ViewModel>();
        public ViewModel CurrentPage { get { return currentPage; } set { SetProperty(ref currentPage, value); } }
        public int CurrentIndex { get { return currentIndex; } set { SetProperty(ref currentIndex, value); } }

        public bool BackButtonVisible { get { return backButtonVisible; } set { SetProperty(ref backButtonVisible, value); } }
    }
}