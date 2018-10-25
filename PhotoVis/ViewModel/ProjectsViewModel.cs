using System.Collections.Generic;
using System.Windows.Input;
using PhotoVis.Models;

namespace PhotoVis.ViewModel
{
    public class ProjectsViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        
        private ICommand _editProjectCommand;
        private ICommand _openProjectCommand;
        private ICommand _deleteProjectCommand;

        #endregion

        #region Properties/Commands

        public string Name
        {
            get { return "Home"; }
        }
        
        public List<ProjectModel> AllProjects
        {
            get
            {
                return ProjectModel.LoadAllProjects();
            }
        }
        
        public ICommand EditProjectCommand
        {
            get
            {
                if (_editProjectCommand == null)
                {
                    _editProjectCommand = new RelayCommand(
                        param => EditProject((ProjectModel)param)
                    );
                }
                return _editProjectCommand;
            }
        }


        public ICommand OpenProjectCommand
        {
            get
            {
                if (_openProjectCommand == null)
                {
                    _openProjectCommand = new RelayCommand(
                        param => OpenProject((ProjectModel)param)
                    );
                }
                return _openProjectCommand;
            }
        }
        
        public ICommand DeleteProjectCommand
        {
            get
            {
                if (_deleteProjectCommand == null)
                {
                    _deleteProjectCommand = new RelayCommand(
                        param => DeleteProject((ProjectModel)param)
                    );
                }
                return _deleteProjectCommand;
            }
        }

        #endregion

        #region Methods

        private void EditProject(ProjectModel model)
        {
            App.VM.OpenEditView(model);
        }

        private void OpenProject(ProjectModel model)
        {
            App.VM.OpenMapView(model);
        }
        
        private void DeleteProject(ProjectModel model)
        {
            model.Delete();
            OnPropertyChanged("AllProjects");
        }

        #endregion
    }
}
