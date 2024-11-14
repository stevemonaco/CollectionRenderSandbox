using Avalonia.Controls;
using Avalonia.Rendering;
using CollectionRenderSandbox.ViewModels;

namespace CollectionRenderSandbox.Views;
public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    public MainWindow()
    {
        InitializeComponent();

        var tl = GetTopLevel(this)!;
        tl.RendererDiagnostics.DebugOverlays = RendererDebugOverlays.Fps | RendererDebugOverlays.RenderTimeGraph | RendererDebugOverlays.LayoutTimeGraph;

    }
}