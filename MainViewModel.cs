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

        public ObservableCollection<int> Results { get; set; } = new ObservableCollection<int>();

        internal async void StartTaskProgressWithResults()
        {
            cancelSource = new CancellationTokenSource();
            Results.Clear();


            var progress = new Progress<dynamic>(value => {
                Results.Add(value.Result);
                ProgressValueResults = value.Percent;
            });

            await Task.Run(() => LoopThroughNumbersWithReporting(100, progress, cancelSource.Token));

            if (cancelSource.IsCancellationRequested)
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
            for (int x = 0; x < numbers; x++)
            {
                if (token.IsCancellationRequested)
                    break;

                Thread.Sleep(300);
                var percent = (x * 100) / numbers;
                var res = x.TwoTimes();

                var result = new { Result = res, Percent = percent };
                progress.Report(result);
            }

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
