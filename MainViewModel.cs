using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TaskExtension
{
    public class MainViewModel : PropChange
    {
        public MainViewModel()
        {
            
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
