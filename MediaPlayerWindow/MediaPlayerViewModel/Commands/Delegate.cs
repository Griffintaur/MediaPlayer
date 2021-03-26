using MediaPlayerViewModel;
using System;
using System.Windows.Forms;
using System.Windows.Input;

namespace MediaPlayerWindow.Commands
{
    public class DelegateCommand : ICommand
    {
        private Action<object> whatToExecute;
        private Func<bool> whenToExecute;
        private MediaPlayerMainWindowViewModel _viewModel;
        public DelegateCommand(Action<object> what, Func<bool> when, MediaPlayerMainWindowViewModel viewModel)
        {
            whatToExecute = what;
            whenToExecute = when;
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return whenToExecute();
        }

        public void Execute(object parameter)
        {
            // throw new NotImplementedException();
            try
            {
                whatToExecute(parameter);
            }
            catch (Exception ex)
            {
                _viewModel.Alert = ex.Message;
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}
