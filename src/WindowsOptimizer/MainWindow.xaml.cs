using System;
using System.Windows;

namespace WindowsOptimizer;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => Scan();
    }

    private void Scan_Click(object sender, RoutedEventArgs e) => Scan();

    private void Scan()
    {
        CpuText.Text = "Monitoring adapter ready";
        RamText.Text = $"{GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024d / 1024 / 1024:0.0} GB visible";
        DiskText.Text = "Monitoring adapter ready";
        Status.Text = "System scan complete. No changes were applied.";
    }

    private void SetPage(string title)
    {
        PageTitle.Text = title;
        Status.Text = $"{title} selected. Review changes before applying them.";
    }

    private void Dashboard_Click(object s, RoutedEventArgs e) => SetPage("Dashboard");
    private void Optimize_Click(object s, RoutedEventArgs e) => SetPage("Optimize");
    private void Debloat_Click(object s, RoutedEventArgs e) => SetPage("Debloat");
    private void Programs_Click(object s, RoutedEventArgs e) => SetPage("Programs Library");
    private void Monitor_Click(object s, RoutedEventArgs e) => SetPage("Hardware Monitor");
    private void Settings_Click(object s, RoutedEventArgs e) => SetPage("Settings");
}
