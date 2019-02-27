using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using PhotoVis.Models;
using SpikeAccountManager;

namespace PhotoVis.ViewModel
{
    public class ApplicationViewModel : ObservableObject
    {
        #region Fields

        private ICommand _changePageCommand;
        //private ICommand _homePageCommand;

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private User _user;

        #endregion

        public ApplicationViewModel()
        {
            // Add available pages
            PageViewModels.Add(new ProjectsViewModel());
            PageViewModels.Add(new NewProjectViewModel());

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }

        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                this._user = value;
                OnPropertyChanged("User");
                OnPropertyChanged("UserDisplayName");
            }
        }

        public string UserDisplayName
        {
            get
            {
                if(this.User != null)
                {
                    return "Welcome " + this.User.FirstName + " " + this.User.LastName;
                }
                else
                {
                    return "";
                }
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        #endregion

        #region Methods

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            if(viewModel is NewProjectViewModel)
            {
                CurrentPageViewModel = new NewProjectViewModel();
            }
            else
            {
                CurrentPageViewModel = PageViewModels
                    .FirstOrDefault(vm => vm == viewModel);
            }
        }


        public void OpenProjectsView()
        {
            CurrentPageViewModel = PageViewModels[0];
        }

        public void OpenEditView(ProjectModel model)
        {
            CurrentPageViewModel = new NewProjectViewModel(model);
        }

        public void OpenMapView(ProjectModel model)
        {
            App.MapVM = new MapViewModel(model);
            CurrentPageViewModel = App.MapVM;
        }

        #endregion
    }
}
