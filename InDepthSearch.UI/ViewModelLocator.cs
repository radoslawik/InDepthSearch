using Autofac;
using InDepthSearch.Core.ViewModels;
using System;
using System.Linq;
using System.Reflection;

namespace InDepthSearch.UI
{
    public class ViewModelLocator : IDisposable
    {
        private readonly IContainer _container;
        public MainWindowViewModel MainWindow { get; }
        public ViewModelLocator()
        {
            var ui = Assembly.GetCallingAssembly();
            var core = Assembly.GetAssembly(typeof(ViewModelBase))!;
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(core).Where(name => name.Name.EndsWith("ViewModel"))
                .AsSelf().InstancePerDependency();
            builder.RegisterAssemblyTypes(core, ui).Where(name => name.Name.EndsWith("Service"))
                .AsImplementedInterfaces().SingleInstance();
            builder.RegisterAssemblyTypes(core, ui).Where(name => name.Name.EndsWith("Manager"))
                .AsImplementedInterfaces().SingleInstance();

            if (Avalonia.Controls.Design.IsDesignMode)
            {
                builder.RegisterAssemblyTypes(core, ui).Where(name => name.Name.EndsWith("Designer")).AsImplementedInterfaces().InstancePerDependency();
            }

            builder.RegisterType<MainWindowViewModel>().AsSelf().SingleInstance();
            _container = builder.Build();
            MainWindow = _container.Resolve<MainWindowViewModel>();

            if (Avalonia.Controls.Design.IsDesignMode)
            {
                // Set parameters needed for designer if any
            }
        }

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
