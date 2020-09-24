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

        /// <summary>
        /// Bound to UI as dynamic
        /// </summary>
        public ObservableCollection<dynamic> Results { get; set; } = new ObservableCollection<dynamic>();

        /// <summary>
        /// Our main work task async
        /// </summary>
        internal async void StartTaskProgressWithResults()
        {
            cancelSource = new CancellationTokenSource();
            Results.Clear();


            var progressReporter = new Progress<dynamic>(value => {
                Results.Add(new { Result = value.Result, Foreground = value.Foreground });
                ProgressValueResults = value.Percent;
            });

            await Task.Run(() => LoopThroughNumbersWithReporting(100, progressReporter, cancelSource.Token, actionFinished, actionError, actionCancel));
        }

        private void actionCancel()
        {
            MessageBox.Show("Action Cancelled");
        }
        private void actionError(Exception obj)
        {
            MessageBox.Show(obj.Message);
        }
        private void actionFinished()
        {
            MessageBox.Show("Finished");
            ProgressValueResults = 0;
        }

        internal async Task<bool> TaskWithErrorHandling(int v, Action<Exception> onError)
        {
            //do some code normally
            try
            {

                await Task.Run(() => { 
                
                

                for (int i = 0; i < 500000000; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"I have written {i}");

                        if (i > 10000)
                            throw new Exception("FUCK");
                }

                });


                if (v < 0)
                    throw new Exception("DUR");

            }
            catch (Exception x)
            {
                onError?.Invoke(x);
            }
            return true;

        }

        /// <summary>
        /// User wants to cancel
        /// </summary>
        internal void ManualCancelProgress()
        {
            cancelSource?.Cancel(true);
        }
       

        private int _progressValue;

        public int ProgressValueResults
        {
            get { return _progressValue; }
            set { _progressValue = value; OnPropChange(); }
        }


        void LoopThroughNumbersWithReporting(int numbers, IProgress<dynamic> progress, CancellationToken token, Action completed, Action<Exception> error, Action cancelled)
        {
            try
            {
                for (int x = 0; x <= numbers; x++)
                {
                    if (token.IsCancellationRequested)
                        return;

                    Thread.Sleep(50);
                    var percent = (x * 100) / numbers;
                    var res = x.TwoTimes();

                    var result = new { Result = res, Percent = percent, Foreground = getBrush() };
                    progress.Report(result);
                }
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
            }
            finally
            {
                if (token.IsCancellationRequested)
                    cancelled?.Invoke();
                else
                    completed?.Invoke();
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

       

        public ObservableCollection<string> DTED { get; set; } = new ObservableCollection<string>();

        internal async void OpenDTEDFiles(IEnumerable<string> fileNames)
        {
            cancelSource = new CancellationTokenSource();

            List<string> dtedHolder = new List<string>();

            Progress<string> dtedProgress = new Progress<string>(value => {

                if (!string.IsNullOrEmpty(value))
                    DTED.Add(value);

                ProgressValueResults = (DTED.Count * 100) / fileNames.Count();
            });
            await Task.Run(() => CreateDTEDHolders(fileNames, dtedProgress, cancelSource.Token));


        }

        private void CreateDTEDHolders(IEnumerable<string> fileNames, IProgress<string> dtedProgress, CancellationToken token)
        {
            Random r = new Random();

            foreach(var file in fileNames)
            {
                if (token.IsCancellationRequested)
                    break;

                Thread.Sleep(r.Next(1, 50));

                dtedProgress.Report(file.ToUpper());
            }
        }


        internal async Task<bool> TaskWithCompletionSource(int v)
        {
            TaskCompletionSource<bool> thisMustMatchSignature = new TaskCompletionSource<bool>();

            //do some code normally, put it in a TRY to catch the errors
            //put in a await Task
            try
            {

           await     Task.Run(() => {

                    for (int i = 0; i < 50000000; i++)
                    {
                        System.Diagnostics.Debug.WriteLine($"I have written {i}");

                        if (i > 5000)
                            throw new Exception("ERROR");
                    }

                });


                if (v < 0)
                    throw new Exception("DUR");

                thisMustMatchSignature.SetResult(true);
            }
            catch (Exception x)
            {
                thisMustMatchSignature.SetException(new Exception("I caught this within TaskWithCompletionSource"));
                System.Diagnostics.Debug.WriteLine(x.Message);
            }

            bool myResult = await thisMustMatchSignature.Task;

            return myResult;
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
