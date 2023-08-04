using System.Reflection;
using Core;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Splat;
using Splat.Microsoft.Extensions.DependencyInjection;
using Splat.ModeDetection;
using UIOverlay.ViewModels;

namespace UIOverlay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            ModeDetector.OverrideModeDetector(Mode.Run);

            ConfigureServices();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            services.UseMicrosoftDependencyResolver();

            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            RegisterUiServices(services);
            Swim.RegisterServices(services);
        }

        private void RegisterUiServices(IServiceCollection services)
        {
            
            services.AddSingleton<OverlayWindowViewModel>(); 
        }
    }
}