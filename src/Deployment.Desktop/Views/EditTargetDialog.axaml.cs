using Avalonia.Controls;
using Deployment.CLI.Localization;
using Deployment.Desktop.Services;
using Deployment.Domain.Entities;

namespace Deployment.Desktop.Views;

public partial class EditTargetDialog : Window
{
    public EditTargetDialog() : this(null!) { }

    public EditTargetDialog(AppServices services)
    {
        InitializeComponent();
        Header.Text = L.T("app.menu.editTarget");
        Title = L.T("app.menu.editTarget");
        NameLabel.Text = L.T("app.target.namePrompt");
        OsLabel.Text = L.T("app.target.osPrompt");
        HostLabel.Text = L.T("app.target.hostPrompt");
        PathLabel.Text = L.T("app.target.pathPrompt");
        ExcludeLabel.Text = L.T("app.target.excludePrompt");
        BrowseButton.Content = L.T("common.browse");
        OkButton.Content = L.T("common.ok");
        CancelButton.Content = L.T("common.cancel");

        OsCombo.ItemsSource = new[] { TargetOS.Windows, TargetOS.Linux };

        if (services is null) return;
        Selector.Configure(services, showRelease: false);
        Selector.SelectionChanged += PopulateFields;
        Loaded += async (_, _) => await Selector.LoadAsync();
    }

    private void PopulateFields()
    {
        var t = Selector.SelectedTarget;
        if (t is null) return;
        NameBox.Text = t.Name;
        OsCombo.SelectedItem = t.OS;
        HostBox.Text = t.Host;
        PathBox.Text = t.DeploymentPath;
        ExcludeBox.Text = t.BackupExcludes;
        ErrorText.IsVisible = false;
    }

    private async void OnBrowseClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var path = await DialogService.PickFolderAsync(this, L.T("app.target.pathPrompt"), PathBox.Text);
        if (path != null) PathBox.Text = path;
    }

    private void OnOkClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var target = Selector.SelectedTarget;
        if (target is null || string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(PathBox.Text))
        {
            ErrorText.Text = L.T("app.target.editValidation");
            ErrorText.IsVisible = true;
            return;
        }

        target.Name = NameBox.Text.Trim();
        target.OS = (TargetOS)OsCombo.SelectedItem!;
        target.Host = string.IsNullOrWhiteSpace(HostBox.Text) ? "localhost" : HostBox.Text.Trim();
        target.DeploymentPath = PathBox.Text.Trim();
        target.BackupExcludes = ExcludeBox.Text?.Trim() ?? string.Empty;

        Close(target);
    }

    private void OnCancelClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) => Close(null);
}
