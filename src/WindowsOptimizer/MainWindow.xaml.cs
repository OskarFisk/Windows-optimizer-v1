using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Threading;

namespace WindowsOptimizer;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromSeconds(1) };
    private readonly Stopwatch _cpuWatch = new();
    private readonly HardwareMonitorService? _hardwareMonitor;
    private TimeSpan _lastCpu;
    private bool _hardwareRefreshRunning;

    private const string RamMapDownloadUrl = "https://live.sysinternals.com/RAMMap.exe";

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MemoryStatusEx
    {
        public uint Length;
        public uint MemoryLoad;
        public ulong TotalPhys;
        public ulong AvailPhys;
        public ulong TotalPageFile;
        public ulong AvailPageFile;
        public ulong TotalVirtual;
        public ulong AvailVirtual;
        public ulong AvailExtendedVirtual;
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            _hardwareMonitor = new HardwareMonitorService();
        }
        catch (Exception ex)
        {
            ActivityText.Text = $"Hardware sensor initialization failed: {ex.Message}";
        }

        Loaded += async (_, _) =>
        {
            Scan();
            _timer.Tick += async (_, _) => await TimerTickAsync();
            _timer.Start();
            await RefreshHardwareAsync();
        };

        Closed += (_, _) =>
        {
            _timer.Stop();
            _hardwareMonitor?.Dispose();
        };

        _lastCpu = Process.GetCurrentProcess().TotalProcessorTime;
        _cpuWatch.Start();
    }

    private void Scan_Click(object sender, RoutedEventArgs e) => Scan();

    private void Scan()
    {
        var memory = GetMemoryStatus();
        var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)!);

        CpuText.Text = "Scanning…";
        RamText.Text = memory is null ? "--" : $"{memory.Value.UsedGb:0.0}/{memory.Value.TotalGb:0.0} GB";
        RamSubText.Text = memory is null ? "physical memory" : $"{memory.Value.Load}% in use";
        DiskText.Text = $"{drive.AvailableFreeSpace / 1024d / 1024 / 1024:0.0} GB";
        UptimeText.Text = FormatUptime(Environment.TickCount64);
        Status.Text = "System scan complete. Live hardware monitoring is active when sensors are available.";
        ActivityText.Text = $"Last scan: {DateTime.Now:HH:mm:ss}  •  {Environment.OSVersion.VersionString}  •  {Environment.ProcessorCount} logical processors";
    }

    private async Task TimerTickAsync()
    {
        try
        {
            var now = Process.GetCurrentProcess().TotalProcessorTime;
            var elapsed = Math.Max(0.001, _cpuWatch.Elapsed.TotalSeconds);
            var cpu = Math.Clamp((now - _lastCpu).TotalSeconds / elapsed / Environment.ProcessorCount * 100, 0, 100);
            _lastCpu = now;
            CpuText.Text = $"{cpu:0}%";
            UptimeText.Text = FormatUptime(Environment.TickCount64);

            var memory = GetMemoryStatus();
            if (memory is not null)
            {
                RamText.Text = $"{memory.Value.UsedGb:0.0}/{memory.Value.TotalGb:0.0} GB";
                RamSubText.Text = $"{memory.Value.Load}% in use";
            }

            await RefreshHardwareAsync();
        }
        catch { }
    }

    private async Task RefreshHardwareAsync()
    {
        if (_hardwareMonitor is null || _hardwareRefreshRunning)
            return;

        _hardwareRefreshRunning = true;
        try
        {
            var snapshot = await Task.Run(() => _hardwareMonitor.Read());

            CpuTempText.Text = snapshot.CpuTemperature.HasValue
                ? $"{snapshot.CpuTemperature.Value:0.0} °C"
                : "--";
            CpuTempSubText.Text = snapshot.CpuTemperature.HasValue
                ? snapshot.CpuName
                : "temperature sensor unavailable";

            GpuTempText.Text = snapshot.GpuTemperature.HasValue
                ? $"{snapshot.GpuTemperature.Value:0.0} °C"
                : "--";
            GpuTempSubText.Text = snapshot.GpuTemperature.HasValue
                ? snapshot.GpuName
                : "GPU temperature unavailable";

            if (snapshot.Fans.Count == 0)
            {
                FanText.Text = "--";
                FanSubText.Text = "No fan RPM sensors available";
            }
            else
            {
                FanText.Text = snapshot.Fans.Count == 1
                    ? $"{snapshot.Fans[0].Rpm:0} RPM"
                    : $"{snapshot.Fans.Count} fans";
                FanSubText.Text = string.Join("  •  ", snapshot.Fans.Select(f => $"{ShortHardwareName(f.HardwareName)}: {f.Rpm:0} RPM"));
            }

            var monitorDetails = new List<string>
            {
                $"CPU: {snapshot.CpuTemperature?.ToString("0.0") ?? "--"} °C  •  {snapshot.CpuName}",
                $"GPU: {snapshot.GpuTemperature?.ToString("0.0") ?? "--"} °C  •  {snapshot.GpuName}"
            };

            monitorDetails.Add(snapshot.Fans.Count == 0
                ? "Fans: no RPM sensors reported by the hardware/driver."
                : "Fans: " + string.Join("  |  ", snapshot.Fans.Select(f => $"{f.SensorName} = {f.Rpm:0} RPM")));

            HardwareDetailsText.Text = string.Join(Environment.NewLine, monitorDetails);
            HardwareStatusText.Text = $"Live sensors • updated {snapshot.Timestamp:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            HardwareStatusText.Text = "Hardware sensors unavailable";
            HardwareDetailsText.Text = $"The monitor could not read hardware sensors. Some sensors require administrator access or a compatible motherboard/GPU driver. {ex.Message}";
        }
        finally
        {
            _hardwareRefreshRunning = false;
        }
    }

    private static string ShortHardwareName(string name)
    {
        if (name.Length <= 22)
            return name;
        return name[..19] + "…";
    }

    private static (double UsedGb, double TotalGb, uint Load)? GetMemoryStatus()
    {
        var status = new MemoryStatusEx { Length = (uint)Marshal.SizeOf<MemoryStatusEx>() };
        if (!GlobalMemoryStatusEx(ref status) || status.TotalPhys == 0)
            return null;

        const double gb = 1024d * 1024d * 1024d;
        return ((status.TotalPhys - status.AvailPhys) / gb, status.TotalPhys / gb, status.MemoryLoad);
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

    private void Programs_Click(object s, RoutedEventArgs e)
    {
        var selector = new ProgramSelectorWindow { Owner = this };
        selector.ShowDialog();
        Status.Text = "Programs Library closed. Selected programs were saved locally.";
        ActivityText.Text = "Program package/manual selection is ready for the installer integration.";
    }

    private void Monitor_Click(object s, RoutedEventArgs e)
    {
        SetPage("Hardware Monitor");
        _ = RefreshHardwareAsync();
    }

    private void Settings_Click(object s, RoutedEventArgs e) => SetPage("Settings");
    private void Ninite_Click(object s, RoutedEventArgs e) => OpenUrl("https://ninite.com/");
    private void Afterburner_Click(object s, RoutedEventArgs e) => OpenUrl("https://www.msi.com/Landing/afterburner/graphics-cards");
    private void RyzenMaster_Click(object s, RoutedEventArgs e) => OpenUrl("https://www.amd.com/en/products/software/ryzen-master.html");
    private void BlueStacks_Click(object s, RoutedEventArgs e) => OpenUrl("https://www.bluestacks.com/");
    private void WindowsUpdate_Click(object s, RoutedEventArgs e) => OpenUrl("ms-settings:windowsupdate");

    private void CleanTemp_Click(object s, RoutedEventArgs e)
    {
        var temp = Path.GetTempPath();
        long removed = 0;
        int count = 0;

        foreach (var file in Directory.EnumerateFiles(temp))
        {
            try
            {
                var len = new FileInfo(file).Length;
                File.Delete(file);
                removed += len;
                count++;
            }
            catch { }
        }

        Status.Text = $"Temporary-file cleanup finished. {count} files removed.";
        ActivityText.Text = $"Cleaned {count} temporary files and approximately {removed / 1024d / 1024:0.0} MB. Locked files were skipped.";
        Scan();
    }

    private void FlushDns_Click(object s, RoutedEventArgs e)
    {
        try
        {
            var psi = new ProcessStartInfo("ipconfig.exe", "/flushdns")
            {
                UseShellExecute = true,
                Verb = "runas",
                CreateNoWindow = true
            };
            Process.Start(psi)?.WaitForExit();
            Status.Text = "DNS cache flushed.";
            ActivityText.Text = "DNS cache refresh requested with administrator elevation.";
        }
        catch (Exception ex)
        {
            Status.Text = "DNS action cancelled or failed.";
            ActivityText.Text = ex.Message;
        }
    }

    private async void RamClean_Click(object s, RoutedEventArgs e)
    {
        try
        {
            Status.Text = "Preparing RAMMap…";
            ActivityText.Text = "Checking for Microsoft Sysinternals RAMMap and preparing the standby-list cleaner.";

            var ramMapPath = await EnsureRamMapAsync();
            if (ramMapPath is null)
            {
                Status.Text = "RAMMap could not be prepared.";
                ActivityText.Text = "Download RAMMap from Microsoft Sysinternals was unsuccessful. No memory changes were made.";
                return;
            }

            var psi = new ProcessStartInfo(ramMapPath, "-Et")
            {
                UseShellExecute = true,
                Verb = "runas",
                WorkingDirectory = Path.GetDirectoryName(ramMapPath)!,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            using var process = Process.Start(psi);
            if (process is null)
                throw new InvalidOperationException("RAMMap could not be started.");

            await process.WaitForExitAsync();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Status.Text = "Standby memory cleanup completed.";
            ActivityText.Text = "Microsoft Sysinternals RAMMap was run with -Et to empty the Windows standby list. The application memory was also collected.";
            Scan();
        }
        catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 1223)
        {
            Status.Text = "RAM cleanup cancelled.";
            ActivityText.Text = "Administrator permission was cancelled. No system memory changes were made.";
        }
        catch (Exception ex)
        {
            Status.Text = "RAM cleanup failed.";
            ActivityText.Text = $"RAMMap could not complete the standby-list cleanup: {ex.Message}";
        }
    }

    private static async Task<string?> EnsureRamMapAsync()
    {
        var toolsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WindowsOptimizer", "tools");
        var ramMapPath = Path.Combine(toolsDirectory, "RAMMap.exe");

        if (File.Exists(ramMapPath))
            return ramMapPath;

        Directory.CreateDirectory(toolsDirectory);

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("WindowsOptimizer/1.0");
            var bytes = await client.GetByteArrayAsync(RamMapDownloadUrl);
            await File.WriteAllBytesAsync(ramMapPath, bytes);

            if (new FileInfo(ramMapPath).Length < 100_000)
            {
                File.Delete(ramMapPath);
                return null;
            }

            return ramMapPath;
        }
        catch
        {
            try { if (File.Exists(ramMapPath)) File.Delete(ramMapPath); } catch { }
            return null;
        }
    }

    private static void OpenUrl(string url)
    {
        try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); } catch { }
    }
}
