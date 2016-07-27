using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayerWindow.Commands
{
    public class DelegateCommand:ICommand
    {
        private Action<object> whatToExecute;
        private Func<bool> whenToExecute;

      public  DelegateCommand(Action<object> what, Func<bool>when  )
        {
            whatToExecute = what;
            whenToExecute = when;
        }

        public bool CanExecute(object parameter)
        {
           return whenToExecute() ;
        }

        public void Execute(object parameter)
        {
           // throw new NotImplementedException();
            
            whatToExecute(parameter );
        }

        public event EventHandler CanExecuteChanged;
    }
}
