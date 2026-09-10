using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace WindowsOptimizer;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly Stopwatch _cpuWatch = new();
    private TimeSpan _lastCpu;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += (_, _) => { Scan(); _timer.Tick += (_, _) => TimerTick(); _timer.Start(); };
        Closed += (_, _) => _timer.Stop();
        _lastCpu = Process.GetCurrentProcess().TotalProcessorTime;
        _cpuWatch.Start();
    }

    private void Scan_Click(object sender, RoutedEventArgs e) => Scan();

    private void Scan()
    {
        var mem = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024d / 1024 / 1024;
        var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)!);
        CpuText.Text = "Scanning…";
        RamText.Text = $"{mem:0.0} GB";
        DiskText.Text = $"{drive.AvailableFreeSpace / 1024d / 1024 / 1024:0.0} GB";
        UptimeText.Text = FormatUptime(Environment.TickCount64);
        Status.Text = "System scan complete. No optimization was applied.";
        ActivityText.Text = $"Last scan: {DateTime.Now:HH:mm:ss}  •  {Environment.OSVersion.VersionString}  •  {Environment.ProcessorCount} logical processors";
    }

    private void TimerTick()
    {
        try
        {
            var now = Process.GetCurrentProcess().TotalProcessorTime;
            var elapsed = Math.Max(0.001, _cpuWatch.Elapsed.TotalSeconds);
            var cpu = Math.Clamp((now - _lastCpu).TotalSeconds / elapsed / Environment.ProcessorCount * 100, 0, 100);
            _lastCpu = now;
            CpuText.Text = $"{cpu:0}%";
            UptimeText.Text = FormatUptime(Environment.TickCount64);
        }
        catch { }
    }

    private static string FormatUptime(long ms)
    {
        var t = TimeSpan.FromMilliseconds(ms);
        return t.TotalDays >= 1 ? $"{(int)t.TotalDays}d {t.Hours:00}h" : $"{t.Hours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }

    private void SetPage(string title)
    {
        PageTitle.Text = title;
        Status.Text = $"{title} selected. Review changes before applying them.";
        ActivityText.Text = $"Opened {title} at {DateTime.Now:HH:mm:ss}.";
    }

    private void Dashboard_Click(object s, RoutedEventArgs e) => SetPage("Dashboard");
    private void Optimize_Click(object s, RoutedEventArgs e) => SetPage("Optimize");
    private void Debloat_Click(object s, RoutedEventArgs e) => SetPage("Debloat");
    private void Programs_Click(object s, RoutedEventArgs e) => Ninite_Click(s, e);
    private void Monitor_Click(object s, RoutedEventArgs e) => SetPage("Hardware Monitor");
    private void Settings_Click(object s, RoutedEventArgs e) => SetPage("Settings");
    private void Ninite_Click(object s, RoutedEventArgs e) => OpenUrl("https://ninite.com/");
    private void Afterburner_Click(object s, RoutedEventArgs e) => OpenUrl("https://www.msi.com/Landing/afterburner/graphics-cards");
    private void RyzenMaster_Click(object s, RoutedEventArgs e) => OpenUrl("https://www.amd.com/en/products/software/ryzen-master.html");
    private void BlueStacks_Click(object s, RoutedEventArgs e) => OpenUrl("https://www.bluestacks.com/");
    private void WindowsUpdate_Click(object s, RoutedEventArgs e) => OpenUrl("ms-settings:windowsupdate");

    private void CleanTemp_Click(object s, RoutedEventArgs e)
    {
        var temp = Path.GetTempPath(); long removed = 0; int count = 0;
        foreach (var file in Directory.EnumerateFiles(temp))
        {
            try { var len = new FileInfo(file).Length; File.Delete(file); removed += len; count++; } catch { }
        }
        Status.Text = $"Temporary-file cleanup finished. {count} files removed.";
        ActivityText.Text = $"Cleaned {count} temporary files and approximately {removed / 1024d / 1024:0.0} MB. Locked files were skipped.";
        Scan();
    }

    private void FlushDns_Click(object s, RoutedEventArgs e)
    {
        try
        {
            var psi = new ProcessStartInfo("ipconfig.exe", "/flushdns") { UseShellExecute = true, Verb = "runas", CreateNoWindow = true };
            Process.Start(psi)?.WaitForExit();
            Status.Text = "DNS cache flushed.";
            ActivityText.Text = "DNS cache refresh requested with administrator elevation.";
        }
        catch (Exception ex) { Status.Text = "DNS action cancelled or failed."; ActivityText.Text = ex.Message; }
    }

    private void RamClean_Click(object s, RoutedEventArgs e)
    {
        GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
        Status.Text = "Application memory trim completed.";
        ActivityText.Text = "Unused managed memory owned by this app was collected. Windows system RAM is not forcibly cleared.";
        Scan();
    }

    private static void OpenUrl(string url)
    {
        try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
    }
}
