using Avalonia.Controls;
using MCServerManager.ViewModels;

namespace MCServerManager.Views;

public partial class ConsoleView : UserControl
{
    private bool _isAtBottom = true;

    public ConsoleView()
    {
        InitializeComponent();
    }

    private void SendCommand()
    {
        var command = CmdBox.Text;
        if (command == string.Empty)
            return;

        if (DataContext is ConsoleViewModel consoleViewModel)
        {
            consoleViewModel.SendCommandCommand.Execute(command);
        }

        CmdBox.Text = string.Empty;
    }

    private void LogScrollList_ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
            return;

        // Detect if the user manually scrolled to the bottom
        double maxOffset = scrollViewer.Extent.Height - scrollViewer.Viewport.Height;

        if (e.ExtentDelta.Length == 0)
        {
            // The user is actively scrolling manually
            _isAtBottom = (maxOffset - scrollViewer.Offset.Y) <= 1.0;
        }
        else if (_isAtBottom && e.ExtentDelta.Y > 0)
        {
            // Content was added, and the user was already at the bottom
            scrollViewer.ScrollToEnd();
        }
    }

    private void CmdBox_KeyDown(object? sender, Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == Avalonia.Input.Key.Enter)
            SendCommand();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SendCommand();
    }
}
