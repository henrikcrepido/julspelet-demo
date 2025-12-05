namespace Julspelet.Maui;

/// <summary>
/// The main application class for the MAUI Hybrid app.
/// Sets the main page to host the Blazor WebView.
/// </summary>
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}
