using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace TransportCompanyIS.Views;

public partial class StatusEditWindow : Window, INotifyPropertyChanged
{
    private string _statusText = string.Empty;

    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }

    public StatusEditWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
