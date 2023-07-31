using System.Reflection;
using ReactiveUI;
using Splat;
using Splat.ModeDetection;

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
            Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        }
    }
}