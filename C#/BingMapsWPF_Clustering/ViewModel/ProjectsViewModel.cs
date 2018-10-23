using System.Collections.Generic;
using System.Windows.Input;
using PhotoVis.Models;

namespace PhotoVis.ViewModel
{
    public class ProjectsViewModel : ObservableObject, IPageViewModel
    {
        #region Fields

        private int _projectId;
        private List<ProjectModel> _allProjects;
        private ProjectModel _currentProject;
        private ICommand _getProjectCommand;
        private ICommand _openProjectCommand;
        private ICommand _saveProjectCommand;

        #endregion

        #region Properties/Commands

        public string Name
        {
            get { return "Home"; }
        }

        public int ProjectId
        {
            get { return _projectId; }
            set
            {
                if (value != _projectId)
                {
                    _projectId = value;
                    OnPropertyChanged("ProjectId");
                }
            }
        }

        public ProjectModel CurrentProject
        {
            get { return _currentProject; }
            set
            {
                if (value != _currentProject)
                {
                    _currentProject = value;
                    OnPropertyChanged("CurrentProject");
                }
            }
        }

        public List<ProjectModel> AllProjects
        {
            get
            {
                return ProjectModel.LoadAllProjects();
            }
        }
        
        public ICommand GetProjectCommand
        {
            get
            {
                if (_getProjectCommand == null)
                {
                    _getProjectCommand = new RelayCommand(
                        param => GetProject(),
                        param => ProjectId > 0
                    );
                }
                return _getProjectCommand;
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

        public ICommand SaveProjectCommand
        {
            get
            {
                if (_saveProjectCommand == null)
                {
                    _saveProjectCommand = new RelayCommand(
                        param => SaveProject(),
                        param => (CurrentProject != null)
                    );
                }
                return _saveProjectCommand;
            }
        }

        #endregion

        #region Methods

        private void GetProject()
        {
            //// Usually you'd get your Product from your datastore,
            //// but for now we'll just return a new object
            //ProjectModel p = new ProjectModel();
            //p.ProjectId = ProjectId;
            //p.ProjectName = "Test Product";
            //CurrentProject = p;
            
            System.Windows.MessageBox.Show("GET");
        }

        private void OpenProject(ProjectModel model)
        {
            App.VM.OpenMapView(model);
        }

        private void SaveProject()
        {
            // You would implement your Product save here
        }

        #endregion
    }
}
