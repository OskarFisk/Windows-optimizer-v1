using System.Diagnostics;
using System.Windows;
using Microsoft.VisualBasic.Devices;

namespace WindowsOptimizer;

public partial class MainWindow : Window
{
    private readonly PerformanceCounter cpu = new("Processor", "% Processor Time", "_Total");
    private readonly PerformanceCounter disk = new("PhysicalDisk", "% Disk Time", "_Total");
    private readonly ComputerInfo computer = new();

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => Scan();
    }

    private void Scan_Click(object sender, RoutedEventArgs e) => Scan();
    private void Scan()
    {
        CpuText.Text = $"{Math.Clamp(cpu.NextValue(), 0, 100):0}%";
        RamText.Text = $"{(computer.TotalPhysicalMemory - computer.AvailablePhysicalMemory) / 1024d / 1024 / 1024:0.0} GB used";
        DiskText.Text = $"{Math.Clamp(disk.NextValue(), 0, 100):0}%";
        Status.Text = "System scan complete. No changes were applied.";
    }
    private void SetPage(string title) { PageTitle.Text = title; Status.Text = $"{title} selected. Review changes before applying them."; }
    private void Dashboard_Click(object s, RoutedEventArgs e) => SetPage("Dashboard");
    private void Optimize_Click(object s, RoutedEventArgs e) => SetPage("Optimize");
    private void Debloat_Click(object s, RoutedEventArgs e) => SetPage("Debloat");
    private void Programs_Click(object s, RoutedEventArgs e) => SetPage("Programs Library");
    private void Monitor_Click(object s, RoutedEventArgs e) => SetPage("Hardware Monitor");
    private void Settings_Click(object s, RoutedEventArgs e) => SetPage("Settings");
}
