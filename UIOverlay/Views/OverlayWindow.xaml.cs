using System;
using System.Reactive.Disposables;
using System.Windows.Interop;
using ReactiveUI;
using Splat;
using UIOverlay.ViewModels;
using Vanara.PInvoke;

namespace UIOverlay.Views
{
    /// <summary>
    /// Interaction logic for OverlayWindow.xaml
    /// </summary>
    public partial class OverlayWindow
    {
        public OverlayWindow()
        {
            InitializeComponent();
            
            ViewModel = Locator.Current.GetService<OverlayWindowViewModel>();
            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(
                        ViewModel,
                        viewModel => viewModel.CurrentWorkSpaceConfig,
                        view => view.WorkSpaceTextBox.Text,
                        workSpace => workSpace?.Name)
                    .DisposeWith(disposableRegistration);
                
                this.OneWayBind(
                        ViewModel,
                        viewModel => viewModel.IsVisible,
                        view => view.StatusBorder.Visibility)
                    .DisposeWith(disposableRegistration);
            });
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var hwnd = new WindowInteropHelper(this).Handle;

            var extendedStyle = User32.GetWindowLong(hwnd, User32.WindowLongFlags.GWL_EXSTYLE);
            User32.SetWindowLong(
                hwnd,
                User32.WindowLongFlags.GWL_EXSTYLE,
                extendedStyle |
                (int)User32.WindowStylesEx.WS_EX_TRANSPARENT |
                (int)User32.WindowStylesEx.WS_EX_LAYERED |
                (int)User32.WindowStylesEx.WS_EX_TOPMOST |
                (int)User32.WindowStylesEx.WS_EX_TOOLWINDOW );
        }
    }
}
