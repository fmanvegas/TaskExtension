using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskExtension
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel VM { get; }

        public MainWindow()
        {
            InitializeComponent();
            if (DataContext is MainViewModel main)
                VM = main;
        }


        private void btnStopReportTask_Click(object sender, RoutedEventArgs e)
        {
            VM.ManualCancelProgress();
        }

        private void btnStopReportTaskWithResults_Click(object sender, RoutedEventArgs e)
        {
            VM.StartTaskProgressWithResults();
        }


        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void btnStartDTED_Click(object sender, RoutedEventArgs e)
        {
            List<string> list = new List<string>();

            for (int x = 0; x < 250; x++)
            {
                list.Add(RandomString(x + 1));
            }


            VM.OpenDTEDFiles(list);
        }
    }
}
