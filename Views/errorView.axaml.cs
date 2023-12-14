using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.PKI.Views;

public partial class errorView : UserControl
{
    public errorView(string errorCode)
    {
        statusLabel = new Label();

        if (errorCode == null || errorCode == "")
        {
            statusLabel.Content = "";
        }
        else
        {
            statusLabel.Content = errorCode;
        }
        
        InitializeComponent();
        
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}