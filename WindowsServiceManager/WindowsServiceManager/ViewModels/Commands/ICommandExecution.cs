using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceManager.ViewModels.Commands
{
    public interface ICommandExecution
    {
        void Execute();
        Task ExecuteAsync();
    }
}
