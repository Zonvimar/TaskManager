using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using TaskManager.ApplicationData;
using TaskManager.Extensions;

namespace TaskManager.Pages
{
    public partial class TasksPage : Page
    {
        public TasksPage()
        {
            InitializeComponent();
            LoadTasks();
            InitializeNotifications();
        }

        private void LoadTasks()
        {
            try
            {
                var tasks = AppConnect.modelOdb.Tasks
                    .Where(t => t.UserID == AppConnect.CurrentUser.UserID)
                    .OrderByDescending(t => t.DueDate)
                    .ToList();
                TasksGrid.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке задач: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeNotifications()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += CheckDueTasks;
            timer.Start();
        }

        private void CheckDueTasks(object sender, EventArgs e)
        {
            var deadline = DateTime.Now.AddMinutes(30);

            var dueTasks = AppConnect.modelOdb.Tasks
                .Where(t => t.UserID == AppConnect.CurrentUser.UserID &&
                           t.DueDate.HasValue &&
                           t.DueDate.Value <= deadline && 
                           t.Status != 3)
                .ToList();

            foreach (var task in dueTasks)
            {
                NotificationExtenstions.ShowTaskDueNotification(task.Title, (DateTime)task.DueDate);
            }
        }


        private void Filter_Changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = AppConnect.modelOdb.Tasks
                .Where(t => t.UserID == AppConnect.CurrentUser.UserID);

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                var searchText = SearchBox.Text.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(searchText) ||
                                       t.Description.ToLower().Contains(searchText));
            }

            if (PriorityFilter.SelectedIndex > 0)
            {
                query = query.Where(t => t.Priority == PriorityFilter.SelectedIndex);
            }

            if (StatusFilter.SelectedIndex > 0)
            {
                query = query.Where(t => t.Status == StatusFilter.SelectedIndex);
            }

            TasksGrid.ItemsSource = query.OrderByDescending(t => t.DueDate).ToList();
        }

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditTaskPage());
        }

        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Tasks task)
            {
                NavigationService.Navigate(new AddEditTaskPage(task));
            }
        }

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Tasks task)
            {
                var result = MessageBox.Show(
                    "Вы уверены что хотите удалить задачу?",
                    "Подтвердить",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        AppConnect.modelOdb.Tasks.Remove(task);
                        AppConnect.modelOdb.SaveChanges();
                        LoadTasks();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка удаления задачи: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ViewStatistics_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StatisticsPage());
        }

        private void TasksGrid_SelectionChanged(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new StatisticsPage());
        }

        private void TasksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
