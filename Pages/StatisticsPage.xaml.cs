using LiveCharts.Wpf;
using LiveCharts;
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
using TaskManager.ApplicationData;

namespace TaskManager.Pages
{
    public partial class StatisticsPage : Page
    {
        public StatisticsPage()
        {
            InitializeComponent();
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            try
            {
                var tasks = AppConnect.modelOdb.Tasks
                    .Where(t => t.UserID == AppConnect.CurrentUser.UserID)
                    .ToList();

                TotalTasksText.Text = tasks.Count.ToString();
                CompletedTasksText.Text = tasks.Count(t => t.Status == 3).ToString();
                PendingTasksText.Text = tasks.Count(t => t.Status != 3).ToString();

                PriorityChart.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "Высокий",
                    Values = new ChartValues<int> { tasks.Count(t => t.Priority == 3) },
                    Fill = new SolidColorBrush(Colors.Red),
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Средний",
                    Values = new ChartValues<int> { tasks.Count(t => t.Priority == 2) },
                    Fill = new SolidColorBrush(Colors.Orange),
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Низкий",
                    Values = new ChartValues<int> { tasks.Count(t => t.Priority == 1) },
                    Fill = new SolidColorBrush(Colors.Green),
                    DataLabels = true
                }
            };

                StatusChart.Series = new SeriesCollection
            {
                new PieSeries
                {
                    Title = "В ожидании",
                    Values = new ChartValues<int> { tasks.Count(t => t.Status == 1) },
                    Fill = new SolidColorBrush(Colors.Gray),
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "В процессе",
                    Values = new ChartValues<int> { tasks.Count(t => t.Status == 2) },
                    Fill = new SolidColorBrush(Colors.Blue),
                    DataLabels = true
                },
                new PieSeries
                {
                    Title = "Выполнена",
                    Values = new ChartValues<int> { tasks.Count(t => t.Status == 3) },
                    Fill = new SolidColorBrush(Colors.Green),
                    DataLabels = true
                }
            };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки статистики: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
