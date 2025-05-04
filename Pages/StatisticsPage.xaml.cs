using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
                var completedStatusId = AppConnect.modelOdb.Statuses
                    .FirstOrDefault(s => s.Name == "Выполнена")?.StatusID;

                // Обновляем числовые показатели
                TotalTasksText.Text = tasks.Count.ToString();
                CompletedTasksText.Text = tasks.Count(t => t.StatusID == completedStatusId).ToString();
                PendingTasksText.Text = tasks.Count(t => t.StatusID != completedStatusId).ToString();

                // Настраиваем график по приоритетам
                var prioritySeries = new SeriesCollection();
                var priorities = AppConnect.modelOdb.Priorities.ToList();

                foreach (var priority in priorities)
                {
                    var count = tasks.Count(t => t.PriorityID == priority.PriorityID);
                    if (count > 0)
                    {
                        prioritySeries.Add(new PieSeries
                        {
                            Title = priority.Name,
                            Values = new ChartValues<int> { count },
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(priority.Color),
                            DataLabels = true
                        });
                    }
                }
                PriorityChart.Series = prioritySeries;

                // Настраиваем график по статусам
                var statusSeries = new SeriesCollection();
                var statuses = AppConnect.modelOdb.Statuses.ToList();

                foreach (var status in statuses)
                {
                    var count = tasks.Count(t => t.StatusID == status.StatusID);
                    if (count > 0)
                    {
                        statusSeries.Add(new PieSeries
                        {
                            Title = status.Name,
                            Values = new ChartValues<int> { count },
                            Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(status.Color),
                            DataLabels = true
                        });
                    }
                }
                StatusChart.Series = statusSeries;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке статистики: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}