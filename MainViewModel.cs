using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TaskExtension
{
    public class MainViewModel : PropChange
    {
        CancellationTokenSource cancelSource = null;


        public MainViewModel()
        {
            
        }

        internal async void StartTaskProgress()
        {
            cancelSource = new CancellationTokenSource();


            var progress = new Progress<int>(percent => {
                Percentage = percent;
            });

            await Task.Run(() => LoopThroughNumbersWithProgress(100, progress, cancelSource.Token));

            if (cancelSource.IsCancellationRequested)
                Output = $"User Cancelled";
            else
                Output = "Completed";

            cancelSource = null;
        }


   
        internal void StartTask()
        {
            int i = 2;

            DoMyTask(i).AwaitVoid(whenVoidCompleted, whenVoidError);
        }
        private void whenVoidCompleted()
        {
            MessageBox.Show("Void Task Completed");
        }
        private void whenVoidError(Exception obj)
        {
            MessageBox.Show($"Void Task Failed {obj.Message}");
        }

        public ObservableCollection<int> Results2 { get; set; } = new ObservableCollection<int>();
        public ObservableCollection<dynamic> Results { get; set; } = new ObservableCollection<dynamic>();

        internal async void StartTaskProgressWithResults()
        {
            cancelSource = new CancellationTokenSource();
            Results.Clear();


            var progress = new Progress<dynamic>(value => {
                Results.Add(new { Result = value.Result, Foreground = value.Foreground });
                ProgressValueResults = value.Percent;
            });

            await Task.Run(() => LoopThroughNumbersWithReporting(100, progress, cancelSource.Token));

            if (cancelSource != null && (bool)cancelSource?.IsCancellationRequested)
                Output = $"User Cancelled";
            else
                Output = "Completed";

            cancelSource = null;

        }

        internal void CancelProgress()
        {
            cancelSource?.Cancel(true);
        }

       


        private int _progressValue;

        public int ProgressValueResults
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropChange(); }
        }
        private int _percentage;

        public int Percentage
        {
            get { return _percentage; }
            set { _percentage = value; OnPropChange(); }
        }


        private string _output = "Nothing";
        public string Output
        {
            get { return _output; }
            set { _output = value; OnPropChange(); }
        }


        private async Task DoMyTask(int i)
        {
            await Task.Run(() =>
            {
                for (int x = 0;x < 30000; x++)
                {
                    Console.WriteLine(x);
                }
                Output = i.TwoTimes().ToString();
            });
        }

        void LoopThroughNumbersWithReporting(int numbers, IProgress<dynamic> progress, CancellationToken token)
        {
            for (int x = 0; x <= numbers; x++)
            {
                if (token.IsCancellationRequested)
                    break;

                Thread.Sleep(50);
                var percent = (x * 100) / numbers;
                var res = x.TwoTimes();
               
                var result = new { Result = res, Percent = percent, Foreground = getBrush() };
                progress.Report(result);
            }

        }

        Brush getBrush()
        {
            Random r = new Random();
            var val = r.Next(100);

            if (val >= 90)
                return Brushes.Red;

            if (val >= 70)
                return Brushes.Orange;

            return Brushes.Green;
        }

        void LoopThroughNumbersWithProgress(int numbers, IProgress<int> progress, CancellationToken token)
        {
            for (int x = 0; x <= numbers; x++)
            {
                if (token.IsCancellationRequested)
                    break;

                Thread.Sleep(10);
                progress.Report((x * 100) / numbers);
            }
        }
    }

    public static class TaskExtender
    {
        public async static void AwaitVoid(this Task task, Action completed, Action<Exception> errored)
        {
            try
            {
                await task;

                completed?.Invoke();
            }
            catch (Exception ex)
            {
                errored?.Invoke(ex);
            }
        }

        public static int TwoTimes(this int i) => i * 2;
    }

    public class PropChange : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropChange([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
