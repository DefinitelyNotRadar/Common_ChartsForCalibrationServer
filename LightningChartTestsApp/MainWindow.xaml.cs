using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Arction.Wpf.Charting;
using Arction.Wpf.Charting.SeriesXY;
using Protocols;
using Settings;

namespace LightningChartTestsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ColorTheme theme;
        private Usrp usrp;

        public MainWindow()
        {
            usrp = new Usrp();
            var deploymentKey = "lgCAALq+gYUUUNQBJABVcGRhdGVhYmxlVGlsbD0yMDE5LTA5LTE5I1JldmlzaW9uPTACgD8RLUj4MtJqUZJYP08cAT2iWwf3J9OtBpdL7i2N+0kh7SWkWVA97OxGhM4wObsk67coGddfPr0up6PC3C0KPwwMCiXxkTBWdZ08iYj+WZzzt0Nh0WCA1IHun718ZKUQZyIfbWo+Zm/ye5a/SYJRwoenYVg95HKI3lUD+tAs9E5lNkeIgHiWrxUpQCFd+lN3d6SVnmaaRwMoLdT6iZF8bbI9drqJKFlcQX2RmV5CHt9ABh2AS4G8AbJMUHJrk4dxbyYxZwINYnlhMJPLtitabW+iK+ZQel62jiAm7jPlCGuWUEX34UUVZkOov5jUOIfYuIlgeDLbVNrcdWXOyy34b/+/D5WBCfBrbbINSOFtti+asDiZzEK6nbPL4FU90A14EOFoY68fsAkM3mdae2V0Kn42zZ0imxz84lfwsYfgjDO7MLwKmeU8YSJQJgRTL4A7bJl72NmDGlBnoPCOvDiTOe3xzEKwBle0yNbQfvvOJ47mwibGxo0gxv/o78RSq0SaaMs=";
            LightningChartUltimate.SetDeploymentKey(deploymentKey);
            InitializeComponent();
            theme = new ColorTheme(new []{Color.FromRgb(220, 50, 0), Color.FromRgb(100, 220, 0), Color.FromRgb(0, 50, 220) }, Color.FromRgb(255,255,255));

            //for (int i = 0; i < Constants.DfReceiversCount * 2; i++)
            //{
            //    var pls = new PointLineSeries();
            //    pls.LineStyle.Color = theme.Colors[i];

            //    c3.ViewXY.PointLineSeries.Add(pls);
            //    pls.MouseClick += (s, a) => { pls.PointsVisible = !pls.PointsVisible;};
            //}

            for (int i = 0; i < 10; i++)
            {
                var pls = new FreeformPointLineSeries();
                pls.LineStyle.Color = theme.Colors[i];

                c3.ViewXY.FreeformPointLineSeries.Add(pls);
                pls.MouseClick += (s, a) => { pls.PointsVisible = !pls.PointsVisible; };
            }

            c3.ViewXY.XAxes[0].AutoFormatLabels = false;
            c3.ViewXY.XAxes[0].LabelsNumberFormat = "0,000,000";
            tb.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    Button_Click_3(sender, new RoutedEventArgs());
            };

            //tbDirection.KeyDown += (sender, args) =>
            //{
            //    if (args.Key == Key.Enter)
            //        Button_Click_3(sender, new RoutedEventArgs());
            //};

            usrp.OnResult += (sender, args) =>
            {
                Dispatcher?.Invoke(() =>
                {
                    labelLog.Content = $"Command result : {args.Result}. Time spent : {args.MillisecondsTaken} ms";
                });
            };
            usrp.OnError += Usrp_OnError;
        }

        private void Usrp_OnError(object sender, string e)
        {
            MessageBox.Show(e);
        }

        private void Read(int index = 0)
        {
            var path1 = $"Data\\asd{index}.bin";
            var path2 = $"Data\\asd{index + 1}.bin";
            var bytes1 = System.IO.File.ReadAllBytes(path1);
            var bytes2 = System.IO.File.ReadAllBytes(path2);
            var data1 = TechAppSpectrumResponse.Parse(bytes1);
            var data2 = TechAppSpectrumResponse.Parse(bytes2);
            Draw(data1, data2);
        }

        private void Draw(TechAppSpectrumResponse response1, TechAppSpectrumResponse response2)
        {
            c3.BeginUpdate();
            //draw first
            var l = response1.Scans[0].Amplitudes.Length;
            for (int i = 0; i < Constants.DfReceiversCount; i++)
            {
                var amps = response1.Scans[i].Amplitudes;
                var pls = c3.ViewXY.PointLineSeries[i];
                pls.Points = new SeriesPoint[amps.Length];
                for (int j = 0; j < amps.Length; j++)
                {
                    pls.Points[j] = new SeriesPoint(j, amps[j]);
                }
            }

            for (int i = 0; i < Constants.DfReceiversCount; i++)
            {
                var amps = response2.Scans[i].Amplitudes;
                var pls = c3.ViewXY.PointLineSeries[i + Constants.DfReceiversCount];
                pls.Points = new SeriesPoint[amps.Length];
                for (int j = 0; j < amps.Length; j++)
                {
                    pls.Points[j] = new SeriesPoint(j, amps[j]);
                }
            }

            c3.ViewXY.XAxes[0].SetRange(0, l);
            c3.EndUpdate();
        }
        public void Smoothing(float[] c)
        {
            var pls0 = c3.ViewXY.PointLineSeries[0];
            pls0.Title.Text = "s";
            var pls1 = c3.ViewXY.PointLineSeries[1];
            pls1.Title.Text = "1";
            var pls2 = c3.ViewXY.PointLineSeries[2];
            pls2.Title.Text = "2";
            var pls3 = c3.ViewXY.PointLineSeries[3];
            pls3.Title.Text = "3";

            pls0.Points = new SeriesPoint[c.Length];
            pls1.Points = new SeriesPoint[c.Length];
            pls2.Points = new SeriesPoint[c.Length];
            pls3.Points = new SeriesPoint[c.Length];

            for (int i = 0; i < c.Length; i++)
            {
                pls0.Points[i] = new SeriesPoint(i, c[i]);
            }

            var N = 10;
            //first part
            var sum = c[0];
            var a = new float[c.Length];
            a[0] = c[0];

            for (int j = 1; j < N; j++)
            {
                sum += c[j];
                a[j] = sum / (j + 1);
            }

            for (int j = N; j < c.Length; j++)
            {
                sum += c[j] - c[j - N];
                a[j] = sum / N;
            }

            for (int i = 0; i < c.Length; i++)
            {
                pls1.Points[i] = new SeriesPoint(i, a[i]);
            }

            //second part
            var output = new float[c.Length];
            output[c.Length - 1] = c[c.Length - 1];
            sum = c[c.Length - 1];
            var n = 1;

            for (int j = c.Length - 2; j > c.Length - (N + 1); j--)
            {
                sum += a[j];
                output[j] = sum / (n + 1);
                n++;
            }

            for (int j = c.Length - (N + 1); j >= 0; j--)
            {
                sum += a[j] - a[j + N];
                output[j] = sum / N;
            }
            //result 
            for (int j = 0; j < c.Length; j++)
            {
                c[j] = a[j];//a[j] / 2 + output[j] / 2;
            }

            for (int i = 0; i < c.Length; i++)
            {
                pls2.Points[i] = new SeriesPoint(i, output[i]);
            }
            for (int i = 0; i < c.Length; i++)
            {
                pls3.Points[i] = new SeriesPoint(i, a[i] / 2 + output[i] / 2);
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            c3.BeginUpdate();
            var args = new float[] { 180, 161, 150, 169, 175, 165, 150, 145, 145, 130, 120, 115, 110, 110, 109, 100, 95, 76, 80, 75, 65, 60, 50, 49, 35, 30, 20, 19, 10, 1 };
            Smoothing(args);
            c3.EndUpdate();
            //Read(index);
            //var b = sender as Button;
            //b.Content = $"Current index {index} and {index + 1}";
            //index += 2;
            //if (index == 100)
            //    index = 0;
        }

        private float[] avgData = new[] { 1.33f, 1.19f, 2.34f, 2.33f, 1.28f, 1.63f, 1.63f, 1.62f, 1.61f, 1.48f, 1.15f, 0.97f};
        private float[] maxData = new[] { 5.73f, 4.85f, 10.9f, 10.9f, 3.8f, 7.0f, 7.0f, 7.8f, 5.7f, 6.0f, 5.0f, 4.16f};

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            c3.BeginUpdate();
            var avg = c3.ViewXY.PointLineSeries[0];
            avg.Points = new SeriesPoint[avgData.Length];


            var max = c3.ViewXY.PointLineSeries[1];
            max.Points = new SeriesPoint[avgData.Length];

            for (int i = 0; i < avgData.Length; i++)
            {
                avg.Points[i] = new SeriesPoint(i + 1, avgData[i]);
                max.Points[i] = new SeriesPoint(i + 1, maxData[i]);
            }

            c3.EndUpdate();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                c3.BeginUpdate();
                var dfData = System.IO.File.ReadAllLines("timedf.txt");
                var noDfData = System.IO.File.ReadAllLines("timenodf.txt");
                c3.Title.Text = "First 5 - direction finding, other 5 no direction finding";
                var pls0 = c3.ViewXY.PointLineSeries[0];
                pls0.Title.Text = "wait";
                var pls1 = c3.ViewXY.PointLineSeries[1];
                pls1.Title.Text = "1 fpga";
                var pls2 = c3.ViewXY.PointLineSeries[2];
                pls2.Title.Text = "2 fpga";
                var pls3 = c3.ViewXY.PointLineSeries[3];
                pls3.Title.Text = "processing";
                var pls4 = c3.ViewXY.PointLineSeries[4];
                pls4.Title.Text = "full";

                pls0.Points = new SeriesPoint[dfData.Length];
                pls1.Points = new SeriesPoint[dfData.Length];
                pls2.Points = new SeriesPoint[dfData.Length];
                pls3.Points = new SeriesPoint[dfData.Length];
                pls4.Points = new SeriesPoint[dfData.Length];


                var pls5 = c3.ViewXY.PointLineSeries[5];
                pls5.Title.Text = "wait";
                var pls6 = c3.ViewXY.PointLineSeries[6];
                pls6.Title.Text = "1 fpga";
                var pls7 = c3.ViewXY.PointLineSeries[7];
                pls7.Title.Text = "2 fpga";
                var pls8 = c3.ViewXY.PointLineSeries[8];
                pls8.Title.Text = "processing";
                var pls9 = c3.ViewXY.PointLineSeries[9];
                pls9.Title.Text = "full";


                pls5.Points = new SeriesPoint[noDfData.Length];
                pls6.Points = new SeriesPoint[noDfData.Length];
                pls7.Points = new SeriesPoint[noDfData.Length];
                pls8.Points = new SeriesPoint[noDfData.Length];
                pls9.Points = new SeriesPoint[noDfData.Length];

                for (int i = 0; i < dfData.Length; i++)
                {
                    var data = Parse(dfData[i]);
                    pls0.Points[i] = new SeriesPoint(i, data[0]);
                    pls1.Points[i] = new SeriesPoint(i, data[1]);
                    pls2.Points[i] = new SeriesPoint(i, data[2]);
                    pls3.Points[i] = new SeriesPoint(i, data[3] - data[0] - data[1] - data[2]);
                    pls4.Points[i] = new SeriesPoint(i, data[3]);
                }

                for (int i = 0; i < noDfData.Length; i++)
                {
                    var data = Parse(noDfData[i]);
                    pls5.Points[i] = new SeriesPoint(i, data[0]);
                    pls6.Points[i] = new SeriesPoint(i, data[1]);
                    pls7.Points[i] = new SeriesPoint(i, data[2]);
                    pls8.Points[i] = new SeriesPoint(i, data[3] - data[0] - data[1] - data[2]);
                    pls9.Points[i] = new SeriesPoint(i, data[3]);
                }
                c3.EndUpdate();
            }
            catch
            {
            }

            float[] Parse(string line)
            {
                var output = new float[4];
                var args = line.Split(' ');
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i][args[i].Length - 1] == ',')
                        args[i] = args[i].Substring(0, args[i].Length - 2);
                }

                output[0] = Single.Parse(args[0]);
                output[1] = Single.Parse(args[1]);
                output[2] = Single.Parse(args[2]);
                output[3] = Single.Parse(args[3]);

                output = output.Select(v => v > 30 ? 30 : v).ToArray();

                return output;
            }
        }

        

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                c3.BeginUpdate();
                var phase = Int32.Parse(tb.Text);
                var direction = Int32.Parse(tbDirection.Text);

                var start = System.IO.File.ReadAllLines("1.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                var refactoring = System.IO.File.ReadAllLines("2.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                var cleaning = System.IO.File.ReadAllLines("3.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                var roundup = System.IO.File.ReadAllLines("4.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                var extrapolation = System.IO.File.ReadAllLines("5.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                var smoothing = System.IO.File.ReadAllLines("6.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                var reversedRefactoring = System.IO.File.ReadAllLines("7.txt").
                    Where(l => l.Split(' ')[0].Equals(direction.ToString()) && l.Split(' ')[1].Equals((phase - 1).ToString())).
                    Select(l => (l.Split(' ')[2], l.Split(' ')[3])).ToArray();
                
                var pls0 = c3.ViewXY.FreeformPointLineSeries[0];
                pls0.Title.Text = "start";
                var pls1 = c3.ViewXY.FreeformPointLineSeries[1];
                pls1.Title.Text = "refactoring";
                var pls2 = c3.ViewXY.FreeformPointLineSeries[2];
                pls2.Title.Text = "cleaning";
                var pls3 = c3.ViewXY.FreeformPointLineSeries[3];
                pls3.Title.Text = "roundup";
                var pls4 = c3.ViewXY.FreeformPointLineSeries[4];
                pls4.Title.Text = "extrapolation";
                var pls5 = c3.ViewXY.FreeformPointLineSeries[5];
                pls5.Title.Text = "smoothing";
                var pls6 = c3.ViewXY.FreeformPointLineSeries[6];
                pls6.Title.Text = "reversedRefactoring";


                var pls8 = c3.ViewXY.FreeformPointLineSeries[8];
                pls8.Title.Text = "median";

                pls0.Points = new SeriesPoint[start.Length];
                pls1.Points = new SeriesPoint[refactoring.Length];
                pls2.Points = new SeriesPoint[cleaning.Length];
                pls3.Points = new SeriesPoint[roundup.Length];
                pls4.Points = new SeriesPoint[extrapolation.Length];
                pls5.Points = new SeriesPoint[smoothing.Length];
                pls6.Points = new SeriesPoint[reversedRefactoring.Length];
                pls8.Points = new SeriesPoint[2];
                
                for (int i = 0 ; i < start.Length; i++)
                    pls0.Points[i] = new SeriesPoint(Int32.Parse(start[i].Item2), Single.Parse(start[i].Item1));

                for (int i = 0; i < refactoring.Length; i++)
                    pls1.Points[i] = new SeriesPoint(Int32.Parse(refactoring[i].Item2), Single.Parse(refactoring[i].Item1));

                for (int i = 0; i < cleaning.Length; i++)
                    pls2.Points[i] = new SeriesPoint(Int32.Parse(cleaning[i].Item2), Single.Parse(cleaning[i].Item1));

                for (int i = 0; i < roundup.Length; i++)
                    pls3.Points[i] = new SeriesPoint(Int32.Parse(roundup[i].Item2), Single.Parse(roundup[i].Item1));

                for (int i = 0; i < extrapolation.Length; i++)
                    pls4.Points[i] = new SeriesPoint(Int32.Parse(extrapolation[i].Item2), Single.Parse(extrapolation[i].Item1));

                for (int i = 0; i < smoothing.Length; i++)
                    pls5.Points[i] = new SeriesPoint(Int32.Parse(smoothing[i].Item2), Single.Parse(smoothing[i].Item1));

                for (int i = 0; i < reversedRefactoring.Length; i++)
                    pls6.Points[i] = new SeriesPoint(Int32.Parse(reversedRefactoring[i].Item2), Single.Parse(reversedRefactoring[i].Item1));
                
                c3.EndUpdate();
            }
            catch
            {
            }
        }

        private List<int[]> Scans = new List<int[]>(4000);

        private async Task RawTask()
        {
            try
            {
                var plsRaw = c3.ViewXY.FreeformPointLineSeries[0];
                plsRaw.Title.Text = "raw";

                for (int i = 0; i < Scans.Count; i++)
                {
                    Dispatcher?.Invoke(() =>
                    {
                        c3.BeginUpdate();

                        plsRaw.Points = new SeriesPoint[Scans[i].Length];

                        for (int j = 0; j < Scans[i].Length; j++)
                        {
                            plsRaw.Points[j] = new SeriesPoint(j, Scans[i][j]);
                        }

                        c3.EndUpdate();
                    });
                    await Task.Delay(10);
                    //break;
                }
            }
            catch { }
        }

        private int Parse(string input)
        {
            if (input.Equals(""))
                return 0;
            return Int32.Parse(input);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Scans.Count == 0)
                {
                    var file = System.IO.File.ReadAllText("raw.txt").Split(' ');
                    var rawData = file.Select(value => Parse(value)).ToArray();
                    var numberOfScans = rawData.Length / 1024;
                    for (int i = 0; i < numberOfScans; i++)
                    {
                        var scan = new int[1024];
                        for (int j = 0; j < 1024; j++)
                        {
                            scan[j] = rawData[j + 1024 * i];
                        }

                        Scans.Add(scan);
                    }
                }

                Task.Run(RawTask);

                //c3.BeginUpdate();
                //fhss part : 
                //var directory = Int32.Parse(tb.Text);
                //var file = Int32.Parse(tbDirection.Text);
                //var directoryExists = System.IO.Directory.Exists($"Slices\\{directory}");
                //if (!directoryExists || !System.IO.File.Exists($"Slices\\{directory}\\slice{file}.txt"))
                //{
                //    MessageBox.Show("Wrong number of measurement");
                //    c3.EndUpdate();
                //    return;
                //}

                //OneFile(directory, file);

                //iq part :
                //var byteScan = System.IO.File.ReadAllBytes("iq.bin");
                //var iqScan = new IqScan(byteScan);

                //var plsRaw = c3.ViewXY.FreeformPointLineSeries[0];
                //plsRaw.Title.Text = "phases";
                //var plsAmplitudes = c3.ViewXY.FreeformPointLineSeries[1];
                //plsAmplitudes.Title.Text = "amplitudes";

                //plsRaw.Points = new SeriesPoint[iqScan.Phases.Length];
                //plsAmplitudes.Points = new SeriesPoint[iqScan.Amplitudes.Length];
                //for (int i = 0; i < iqScan.Phases.Length; i++)
                //{
                //    plsRaw.Points[i] = new SeriesPoint(i, iqScan.Phases[i]);
                //}

                //for (int i = 0; i < iqScan.Amplitudes.Length; i++)
                //{
                //    plsAmplitudes.Points[i] = new SeriesPoint(i, iqScan.Amplitudes[i]);
                //}

                //c3.EndUpdate();
            }
            catch(Exception exx)
            {
                var pls = exx.StackTrace;
            }

            void AllFiles(int directory)
            {
                var files = System.IO.Directory.GetFiles($"Slices\\{directory}\\");
                var resultstr = "";

                for (int i = 0; i < files.Length && i < c3.ViewXY.FreeformPointLineSeries.Count; i++)
                {
                    resultstr += $"file {i}:\r\n";
                    var fileSignals = System.IO.File.ReadAllLines(files[i]).Select(l => l.Split(' '))
                        .Select(s => new Signal(s)).ToList();
                    var pls = c3.ViewXY.FreeformPointLineSeries[i];
                    pls.Title.Text = $"{files[i].Split('\\').Last()}";
                    pls.LineVisible = false;
                    pls.PointsVisible = true;
                    pls.PointStyle.Color1 = theme.Colors[i];
                    pls.PointStyle.Color2 = theme.Colors[i];
                    pls.PointStyle.Color3 = theme.Colors[i];
                    pls.Points = new SeriesPoint[fileSignals.Count];

                    for (int j = 0; j < fileSignals.Count; j++)
                        pls.Points[j] = new SeriesPoint(fileSignals[j].FrequencyKhz, fileSignals[j].Direction);

                    var result = TestClass.Test(fileSignals);

                    foreach (var r in result)
                    {
                        resultstr += $"\t{r.StartFrequencyKhz} - {r.EndFrequencyKhz} : {r.MedianDirection}. N - {r.Count}\r\n";
                    }
                }
                System.IO.File.WriteAllText("searchResults.txt", resultstr);
            }

            void OneFile(int directory, int fileIndex)
            {
                var resultstr = $"file {fileIndex}:\r\n";
                var fileSignals = System.IO.File.ReadAllLines($"Slices\\{directory}\\slice{fileIndex}.txt").Select(l => l.Split(' '))
                    .Select(s => new Signal(s)).ToList();
                var i = 0;
                var pls = c3.ViewXY.FreeformPointLineSeries[i];
                pls.LineVisible = false;
                pls.PointsVisible = true;
                pls.Title.Text = $"pointz";
                pls.PointStyle.Color1 = theme.Colors[i];
                pls.PointStyle.Color2 = theme.Colors[i];
                pls.PointStyle.Color3 = theme.Colors[i];
                pls.Points = new SeriesPoint[fileSignals.Count];

                for (int j = 0; j < fileSignals.Count; j++)
                    pls.Points[j] = new SeriesPoint(fileSignals[j].FrequencyKhz, fileSignals[j].Direction);

                var result = TestClass.Test(fileSignals);
                var c = 1;
                foreach (var r in result)
                {
                    resultstr += $"\t{r.StartFrequencyKhz} - {r.EndFrequencyKhz} : {r.MedianDirection}. N - {r.Count}\r\n";
                    if(r.Count < 50)
                        continue;
                    var plsc = c3.ViewXY.FreeformPointLineSeries[c];
                    c++;
                    plsc.LineVisible = true;
                    plsc.PointsVisible = false;
                    plsc.PointStyle.Color1 = theme.Colors[c];
                    plsc.PointStyle.Color2 = theme.Colors[c];
                    plsc.PointStyle.Color3 = theme.Colors[c];
                    plsc.Title.Text = $"{c}";
                    plsc.Points = new SeriesPoint[2];
                    plsc.Points[0] = new SeriesPoint(r.StartFrequencyKhz, r.MedianDirection);
                    plsc.Points[1] = new SeriesPoint(r.EndFrequencyKhz, r.MedianDirection);
                }
                System.IO.File.WriteAllText("searchResults.txt", resultstr);
            }
        }

        private bool _isUsrpOpen = false;

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (_isUsrpOpen)
            {
                usrp.Close();
            }
            else
            {
                usrp.OpenAndInit();
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            c3.BeginUpdate();
            var data = usrp.Read();
            var numberOfPoints = data.Length / data.Rank;
            for (int i = 0; i < data.Rank; i++)
            {
                var pls = c3.ViewXY.FreeformPointLineSeries[i];
                pls.LineVisible = true;
                pls.PointsVisible = false;
                pls.Title.Text = $"channel {i + 1}";
                pls.PointStyle.Color1 = theme.Colors[i];
                pls.PointStyle.Color2 = theme.Colors[i];
                pls.PointStyle.Color3 = theme.Colors[i];
                pls.Points = new SeriesPoint[numberOfPoints];
                for (int j = 0; j < numberOfPoints; j++)
                {
                    pls.Points[j] = new SeriesPoint(j, data[i,j]);
                }
            }
            c3.EndUpdate();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            try
            {
                var frequencyMhz = Int32.Parse(tbDirection.Text);
                usrp.SetFrequency(frequencyMhz);
            }
            catch
            {
                labelLog.Content = "Exception during set frequency";
            }
        }

        private async Task LoopTask(CancellationToken token)
        {
        }
    }
}
