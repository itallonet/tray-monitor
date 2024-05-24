using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace tray_monitor;

public partial class TrayApp : Form
{
    private NotifyIcon _trayIcon;
    private Timer _updateTimer;
    private PerformanceCounter _cpuCounter;
    private PerformanceCounter _memoryCounter;

    public TrayApp()
    {
        try
        {
            // Set Icon
            _trayIcon = new NotifyIcon()
            {
                Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Icons", "app.ico"), 40, 40),
                Visible = true
            };

            _trayIcon.MouseMove += OnMouseMove;
            _trayIcon.MouseClick += OnMouseClick;


            // Set Performance Counters
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memoryCounter = new PerformanceCounter("Memory", "Available MBytes");

            // Set Timer
            _updateTimer = new Timer
            {
                Interval = 1000
            };
            _updateTimer.Tick += UpdateData;
            _updateTimer.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(1); // Encerra o aplicativo com código de erro
        }
        InitializeComponent();

    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_cpuCounter != null && _memoryCounter != null)
        {
            float CPUUsage = _cpuCounter.NextValue();
            float AvailableMemory = _memoryCounter.NextValue();
            string Tooltip = $"CPU: {CPUUsage:F1}%\nMemory: {AvailableMemory}MB\nDisk: {GetDiskUsage()}%";
            _trayIcon.Text = Tooltip;
        }
    }

    private void OnMouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            Application.Exit();
        }
        else
        {
            // Open task manager
            string TaskManagerPath = Environment.ExpandEnvironmentVariables(@"%windir%\system32\taskmgr.exe");
            Process.Start(TaskManagerPath);
        }
    }

    private void UpdateData(object sender, EventArgs e)
    {
        // Refresh tooltip
        float CPUUsage = _cpuCounter.NextValue();
        float AvailableMemory = _memoryCounter.NextValue();
        string tooltipText = $"CPU: {CPUUsage:F1}%\nRAM: {AvailableMemory}MB\nDisco: {GetDiskUsage()}%";
        _trayIcon.Text = tooltipText;
    }


    private float GetDiskUsage()
    {
        var DiskCounter = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
        return DiskCounter.NextValue();
    }

    protected override void OnLoad(EventArgs e)
    {
        Visible = false; // Hide main window
        ShowInTaskbar = false; // Hide task bar icon
        base.OnLoad(e);
    }
}
