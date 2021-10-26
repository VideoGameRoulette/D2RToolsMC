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
        private DispatcherTimer[] countdownTimer;
        private DispatcherTimer refreshTimer;

        public MainWindow()
        {
            InitializeComponent();
            countdownTimer = new DispatcherTimer[8];
            for (var idx = 0; idx < countdownTimer.Length; idx++) countdownTimer[idx] = new DispatcherTimer();

            var i = 0;
            foreach (DispatcherTimer dt in countdownTimer)
            {
                i++;
                dt.Interval = TimeSpan.FromSeconds(1);
                switch(i)
                {
                    case 1:
                        dt.Tick += countdownTimer_Tick;
                        break;
                    case 2:
                        dt.Tick += countdownTimer2_Tick;
                        break;
                    case 3:
                        dt.Tick += countdownTimer3_Tick;
                        break;
                    case 4:
                        dt.Tick += countdownTimer4_Tick;
                        break;
                    case 5:
                        dt.Tick += countdownTimer5_Tick;
                        break;
                    case 6:
                        dt.Tick += countdownTimer6_Tick;
                        break;
                    case 7:
                        dt.Tick += countdownTimer7_Tick;
                        break;
                    case 8:
                        dt.Tick += countdownTimer8_Tick;
                        break;
                }
                
            }

            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(1);
            refreshTimer.Tick += refreshTimer_Tick;
        }

        private Process[] GetProcess() => Process.GetProcessesByName("d2r");
        private Process[] gameProcess;

        //public int currentTime;

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

        private int[] currentTime = { 30, 30, 30, 30, 30, 30, 30, 30 };

        public void GetServerDetails()
        {
            if (gameProcess == default) gameProcess = GetProcess();
            if (gameProcess.Length > 0)
            {
                for (var i = 0; i < gameProcess.Length; i++)
                {
                    SetTitle(i);
                    GetClientDetails(i);
                }
            }
        }

        public void SetTitle(int i)
        {
            switch(i)
            {
                case 1:
                    ClientName2.Text = gameProcess[i].MainWindowTitle;
                    return;
                case 2:
                    ClientName3.Text = gameProcess[i].MainWindowTitle;
                    return;
                case 3:
                    ClientName4.Text = gameProcess[i].MainWindowTitle;
                    return;
                case 4:
                    ClientName5.Text = gameProcess[i].MainWindowTitle;
                    return;
                case 5:
                    ClientName6.Text = gameProcess[i].MainWindowTitle;
                    return;
                case 6:
                    ClientName7.Text = gameProcess[i].MainWindowTitle;
                    return;
                case 7:
                    ClientName8.Text = gameProcess[i].MainWindowTitle;
                    return;
                default:
                    ClientName1.Text = gameProcess[i].MainWindowTitle;
                    return;
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
                            StartTimer(i);
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
            SetTextColor(0, CurrentIP);
            CurrentIP2.Text = ips[1];
            SetTextColor(1, CurrentIP2);
            CurrentIP3.Text = ips[2];
            SetTextColor(2, CurrentIP3);
            CurrentIP4.Text = ips[3];
            SetTextColor(3, CurrentIP4);
            CurrentIP5.Text = ips[4];
            SetTextColor(4, CurrentIP5);
            CurrentIP6.Text = ips[5];
            SetTextColor(5, CurrentIP6);
            CurrentIP7.Text = ips[6];
            SetTextColor(6, CurrentIP7);
            CurrentIP8.Text = ips[7];
            SetTextColor(7, CurrentIP8);
        }

        private void SetTextColor(int i, System.Windows.Controls.TextBlock tb)
        {
            string[] args = SearchBar.Text.Split(',');
            if (args.Length > 1)
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

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            GetServerDetails();
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

        private void countdownTimer_Tick(object sender, EventArgs e)
        {
            if (currentTime[0] > 0)
            {
                currentTime[0] -= 1;
                SetTime(currentTime[0], CountdownLabel);
            }
            else
            {
                SetTime(0, CountdownLabel);
                countdownTimer[0].Stop();
            }
        }

        private void countdownTimer2_Tick(object sender, EventArgs e)
        {
            if (currentTime[1] > 0)
            {
                currentTime[1] -= 1;
                SetTime(currentTime[1], CountdownLabel2);
            }
            else
            {
                SetTime(0, CountdownLabel2);
                countdownTimer[1].Stop();
            }
        }

        private void countdownTimer3_Tick(object sender, EventArgs e)
        {
            if (currentTime[2] > 0)
            {
                currentTime[2] -= 1;
                SetTime(currentTime[2], CountdownLabel3);
            }
            else
            {
                SetTime(0, CountdownLabel3);
                countdownTimer[2].Stop();
            }
        }

        private void countdownTimer4_Tick(object sender, EventArgs e)
        {
            if (currentTime[3] > 0)
            {
                currentTime[3] -= 1;
                SetTime(currentTime[3], CountdownLabel4);
            }
            else
            {
                SetTime(0, CountdownLabel4);
                countdownTimer[3].Stop();
            }
        }

        private void countdownTimer5_Tick(object sender, EventArgs e)
        {
            if (currentTime[4] > 0)
            {
                currentTime[4] -= 1;
                SetTime(currentTime[4], CountdownLabel5);
            }
            else
            {
                SetTime(0, CountdownLabel5);
                countdownTimer[4].Stop();
            }
        }

        private void countdownTimer6_Tick(object sender, EventArgs e)
        {
            if (currentTime[5] > 0)
            {
                currentTime[5] -= 1;
                SetTime(currentTime[5], CountdownLabel6);
            }
            else
            {
                SetTime(0, CountdownLabel6);
                countdownTimer[5].Stop();
            }
        }

        private void countdownTimer7_Tick(object sender, EventArgs e)
        {
            if (currentTime[6] > 0)
            {
                currentTime[6] -= 1;
                SetTime(currentTime[6], CountdownLabel7);
            }
            else
            {
                SetTime(0, CountdownLabel7);
                countdownTimer[6].Stop();
            }
        }

        private void countdownTimer8_Tick(object sender, EventArgs e)
        {
            if (currentTime[7] > 0)
            {
                currentTime[7] -= 1;
                SetTime(currentTime[7], CountdownLabel8);
            }
            else
            {
                SetTime(0, CountdownLabel8);
                countdownTimer[7].Stop();
            }
        }

        private void StartTimer(int i)
        {
            currentTime[i] = ConvertStringToInt(CustomCooldown.Text);
            
            switch (i)
            {
                case 1:
                    SetTime(currentTime[i], CountdownLabel);
                    break;
                case 2:
                    SetTime(currentTime[i], CountdownLabel2);
                    break;
                case 3:
                    SetTime(currentTime[i], CountdownLabel3);
                    break;
                case 4:
                    SetTime(currentTime[i], CountdownLabel4);
                    break;
                case 5:
                    SetTime(currentTime[i], CountdownLabel5);
                    break;
                case 6:
                    SetTime(currentTime[i], CountdownLabel6);
                    break;
                case 7:
                    SetTime(currentTime[i], CountdownLabel7);
                    break;
                default:
                    SetTime(currentTime[i], CountdownLabel8);
                    break;
            }
            countdownTimer[i].Start();
        }

        private void SetTime(int i, System.Windows.Controls.TextBlock tb)
        {
            tb.Text = string.Format("{0}", i);
        }

        private void RestartTimer_Click(object sender, EventArgs e)
        {
            StartTimer(0);
        }

        private void RestartTimer2_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(1);
        }

        private void RestartTimer3_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(2);
        }

        private void RestartTimer4_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(3);
        }

        private void RestartTimer5_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(4);
        }

        private void RestartTimer6_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(5);
        }

        private void RestartTimer7_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(6);
        }

        private void RestartTimer8_Click(object sender, RoutedEventArgs e)
        {
            StartTimer(7);
        }

        private void CopyIP1_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(CurrentIP.Text);
        }

        private void CopyIP2_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP2.Text);
        }

        private void CopyIP3_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP3.Text);
        }

        private void CopyIP4_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP4.Text);
        }

        private void CopyIP5_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP5.Text);
        }

        private void CopyIP6_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP6.Text);
        }

        private void CopyIP7_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP7.Text);
        }

        private void CopyIP8_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentIP8.Text);
        }
    }
}
