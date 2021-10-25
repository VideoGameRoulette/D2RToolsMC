using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace D2RTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer countdownTimer;
        private DispatcherTimer refreshTimer;

        public MainWindow()
        {
            InitializeComponent();
            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = TimeSpan.FromSeconds(1);
            countdownTimer.Tick += countdownTimer_Tick;

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(1);
            refreshTimer.Tick += refreshTimer_Tick;
        }

        private Process[] GetProcess() => Process.GetProcessesByName("d2r");
        private Process[] gameProcess;

        public int currentTime;

        private string[] FilteredIP = {
            "24.105.29.76", // GLOBAL??
            "34.117.122.6", // GLOBAL??
            "37.244.28.", // US
            "37.244.54.", // US
            "117.52.35.", // ASIA
            "127.0.0.1", // LOCAL
            "137.221.105.", // EU
            "137.221.106." // EU
        };

        private string[] ips = { "0.0.0.0", "0.0.0.0", "0.0.0.0", "0.0.0.0", "0.0.0.0", "0.0.0.0", "0.0.0.0", "0.0.0.0" };

        public void GetServerDetails()
        {
            if (gameProcess == default) gameProcess = GetProcess();
            if (gameProcess.Length > 0)
            {
                for (var i = 0; i < gameProcess.Length; i++) GetClientDetails(i);
            }
        }

        public void GetClientDetails(int i)
        {
            try
            {
                using (Process p = new Process())
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-c -n d2r";
                    ps.FileName = "tcpvcon64.exe";
                    if (!File.Exists(ps.FileName))
                    {
                        checkBox2.IsChecked = false;
                        return;
                    }
                    ps.UseShellExecute = false;
                    ps.CreateNoWindow = true;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;

                    p.StartInfo = ps;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    //Get The Rows
                    string[] lines = Regex.Split(content, "\r\n");

                    List<string> IPList = new List<string>();

                    if (gameProcess == default) gameProcess = GetProcess();

                    foreach (string line in lines)
                    {
                        if (line.Contains("ESTABLISHED"))
                        {
                            if (!line.Contains(FilteredIP[0]) &&
                                !line.Contains(FilteredIP[1]) &&
                                !line.Contains(FilteredIP[2]) &&
                                !line.Contains(FilteredIP[3]) &&
                                !line.Contains(FilteredIP[4]) &&
                                !line.Contains(FilteredIP[5]) &&
                                !line.Contains(FilteredIP[6]) &&
                                !line.Contains(FilteredIP[7]) &&
                                line.Contains(gameProcess[i].Id.ToString()))
                            {
                                IPList.Add(line);
                            }
                        }
                    }

                    if (IPList.Count > 0)
                    {
                        string[] currentServerData = Regex.Split(IPList.Last(), ",");
                        if (ips[i] != currentServerData.Last())
                        {
                            ips[i] = currentServerData.Last();
                            StartTimer();
                        }
                        UpdateIP();
                    }
                    else if (IPList.Count == 0)
                    {
                        ips[i] = "0.0.0.0";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateIP()
        {
            CurrentIP.Text = ips[0];
            SetTextColor(CurrentIP);
            CurrentIP2.Text = ips[1];
            SetTextColor(CurrentIP2);
            CurrentIP3.Text = ips[2];
            SetTextColor(CurrentIP3);
            CurrentIP4.Text = ips[3];
            SetTextColor(CurrentIP4);
            CurrentIP5.Text = ips[4];
            SetTextColor(CurrentIP5);
            CurrentIP6.Text = ips[5];
            SetTextColor(CurrentIP6);
            CurrentIP7.Text = ips[6];
            SetTextColor(CurrentIP7);
            CurrentIP8.Text = ips[7];
            SetTextColor(CurrentIP8);
        }

        private void SetTextColor(System.Windows.Controls.TextBlock tb)
        {
            string[] args = SearchBar.Text.Split(',');
            if (args.Length > 1)
            {
                for (var i = 0; i == args.Length; i++)
                {
                    if (i == args.Length)
                    {
                        tb.Foreground = Brushes.Red;
                        return;
                    }
                    if (args[i] == tb.Text)
                    {
                        tb.Foreground = Brushes.Green;
                        return;
                    }
                }

            }
            else
            {
                if (SearchBar.Text != string.Empty && SearchBar.Text != "0.0.0.0" && SearchBar.Text == CurrentIP.Text)
                {
                    tb.Foreground = Brushes.Green;
                    return;
                }
                else if (SearchBar.Text != string.Empty && SearchBar.Text != "0.0.0.0" && SearchBar.Text != CurrentIP.Text)
                {
                    tb.Foreground = Brushes.Red;
                    return;
                }
                else
                {
                    tb.Foreground = Brushes.White;
                    return;
                }
            }
            
        }

        private void FetchIP_Click(object sender, EventArgs e)
        {
            GetServerDetails();
        }

        private void CopyIP_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(CurrentIP.Text);
        }

        private void StartTimer()
        {
            currentTime = ConvertStringToInt(CustomCooldown.Text);
            SetTime(currentTime);
            countdownTimer.Start();
        }

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            if (currentTime > 0)
            {
                currentTime -= 1;
                SetTime(currentTime);
            }
            else
            {
                SetTime(0);
                countdownTimer.Stop();
            }
        }

        private void SetTime(int i)
        {
            CountdownLabel.Text = string.Format("{0}", i);
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            GetServerDetails();
        }

        private void RestartTimer_Click(object sender, EventArgs e)
        {
            StartTimer();
        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (checkBox2.IsChecked == true) refreshTimer.Start();
            else refreshTimer.Stop();
        }

        private int ConvertStringToInt(string s)
        {
            int i = 0;
            var result = int.TryParse(s, out i);
            if (result) return i;
            return 0;
        }

    }
}
