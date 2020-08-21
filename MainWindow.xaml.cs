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

        private void btnDoTask_Click(object sender, RoutedEventArgs e)
        {
            VM.StartTask();
        }

        private void btnDoReportTask_Click(object sender, RoutedEventArgs e)
        {
            VM.StartTaskProgress();
        }

        private void btnStopReportTask_Click(object sender, RoutedEventArgs e)
        {
            VM.CancelProgress();
        }

        private void btnStopReportTaskWithResults_Click(object sender, RoutedEventArgs e)
        {
            VM.StartTaskProgressWithResults();
        }
    }
}
