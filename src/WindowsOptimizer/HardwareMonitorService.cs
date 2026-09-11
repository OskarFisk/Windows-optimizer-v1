using LibreHardwareMonitor.Hardware;

namespace WindowsOptimizer;

public sealed class HardwareMonitorService : IDisposable
{
    private readonly Computer _computer;
    private readonly object _sync = new();
    private bool _disposed;

    public HardwareMonitorService()
    {
        _computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true,
            IsMemoryEnabled = true,
            IsStorageEnabled = false,
            IsNetworkEnabled = false,
            IsPowerMonitorEnabled = false
        };

        _computer.Open();
    }

    public HardwareSnapshot Read()
    {
        lock (_sync)
        {
            ThrowIfDisposed();

            try
            {
                _computer.Accept(new UpdateVisitor());
            }
            catch
            {
                // Some sensor drivers can fail independently. Continue with the
                // sensors that remain available instead of taking down the UI.
            }

            var hardware = EnumerateHardware().ToList();
            var cpu = hardware.FirstOrDefault(h => h.HardwareType == HardwareType.Cpu);
            var gpus = hardware.Where(IsGpu).ToList();

            var cpuTemperature = FindCpuTemperature(cpu);
            var gpuTemperature = FindGpuTemperature(gpus);
            var fans = FindFans(hardware);

            var cpuName = cpu?.Name ?? "CPU";
            var gpuName = gpus.FirstOrDefault()?.Name ?? "GPU";

            return new HardwareSnapshot(
                cpuName,
                cpuTemperature,
                gpuName,
                gpuTemperature,
                fans,
                DateTime.Now);
        }
    }

    private IEnumerable<IHardware> EnumerateHardware()
    {
        foreach (var hardware in _computer.Hardware)
        {
            yield return hardware;
            foreach (var subHardware in EnumerateSubHardware(hardware))
                yield return subHardware;
        }
    }

    private static IEnumerable<IHardware> EnumerateSubHardware(IHardware hardware)
    {
        foreach (var subHardware in hardware.SubHardware)
        {
            yield return subHardware;
            foreach (var nested in EnumerateSubHardware(subHardware))
                yield return nested;
        }
    }

    private static bool IsGpu(IHardware hardware) =>
        hardware.HardwareType is HardwareType.GpuNvidia
            or HardwareType.GpuAmd
            or HardwareType.GpuIntel;

    private static double? FindCpuTemperature(IHardware? cpu)
    {
        if (cpu is null)
            return null;

        var temperatures = cpu.Sensors
            .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
            .ToList();

        var preferred = temperatures.FirstOrDefault(s =>
            s.Name.Contains("Package", StringComparison.OrdinalIgnoreCase) ||
            s.Name.Contains("Tctl", StringComparison.OrdinalIgnoreCase) ||
            s.Name.Contains("Tdie", StringComparison.OrdinalIgnoreCase) ||
            s.Name.Contains("CPU", StringComparison.OrdinalIgnoreCase));

        return (preferred ?? temperatures.FirstOrDefault())?.Value;
    }

    private static double? FindGpuTemperature(IEnumerable<IHardware> gpus)
    {
        var temperatures = gpus
            .SelectMany(h => h.Sensors)
            .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
            .ToList();

        var preferred = temperatures.FirstOrDefault(s =>
            s.Name.Contains("GPU Core", StringComparison.OrdinalIgnoreCase) ||
            s.Name.Contains("Core", StringComparison.OrdinalIgnoreCase));

        return (preferred ?? temperatures.FirstOrDefault())?.Value;
    }

    private static IReadOnlyList<FanReading> FindFans(IEnumerable<IHardware> hardware)
    {
        return hardware
            .SelectMany(h => h.Sensors.Select(s => (Hardware: h, Sensor: s)))
            .Where(x => x.Sensor.SensorType == SensorType.Fan && x.Sensor.Value.HasValue && x.Sensor.Value.Value >= 0)
            .Select(x => new FanReading(x.Hardware.Name, x.Sensor.Name, x.Sensor.Value!.Value))
            .GroupBy(x => $"{x.HardwareName}|{x.SensorName}", StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(x => x.HardwareName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(x => x.SensorName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HardwareMonitorService));
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed)
                return;

            _disposed = true;
            try { _computer.Close(); } catch { }
        }
    }

    private sealed class UpdateVisitor : IVisitor
    {
        public void VisitComputer(IComputer computer) => computer.Traverse(this);

        public void VisitHardware(IHardware hardware)
        {
            try { hardware.Update(); } catch { }
            foreach (var subHardware in hardware.SubHardware)
                subHardware.Accept(this);
        }

        public void VisitSensor(ISensor sensor) { }
        public void VisitParameter(IParameter parameter) { }
    }
}

public sealed record HardwareSnapshot(
    string CpuName,
    double? CpuTemperature,
    string GpuName,
    double? GpuTemperature,
    IReadOnlyList<FanReading> Fans,
    DateTime Timestamp);

public sealed record FanReading(string HardwareName, string SensorName, float Rpm);
