using System;
using System.Reactive.Disposables;
using System.Windows.Interop;
using Core;
using Domain;
using ReactiveUI;
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
            var desktopService = new DesktopService();

            var wm = new Swim(desktopService, null);

            ViewModel = new OverlayWindowViewModel(wm);
            InitializeComponent();
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

            /*
            HotkeyManager.Current.AddOrReplace("NextWs", Key.Y, ModifierKeys.Control | ModifierKeys.Alt,
                (s, e) => wm.NextWorkSpace());
            HotkeyManager.Current.AddOrReplace("PrevWs", Key.U, ModifierKeys.Control | ModifierKeys.Alt,
                (s, e) => wm.PrevWorkSpace());
            HotkeyManager.Current.AddOrReplace("Close", Key.W, ModifierKeys.Control | ModifierKeys.Alt,
                (s, e) => desktopService.GetForegroundWindow().MatchSome(w => w.Close()));
                */
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
