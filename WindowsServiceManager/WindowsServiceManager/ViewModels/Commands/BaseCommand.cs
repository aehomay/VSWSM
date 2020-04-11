﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceManager.Model;

namespace WindowsServiceManager.ViewModels.Commands
{
    public abstract class BaseCommand
    {
        protected readonly WindowsServiceViewModel ViewMode;

        public Dictionary<WindowsServiceInfo, ServiceController> Controllers
        {
            get
            {
                return ViewMode.SelectedItems;
            }
        }

        public BaseCommand(WindowsServiceViewModel viewModel)
        {
            ViewMode = viewModel;
        }

        public abstract void Execute();
    }
}