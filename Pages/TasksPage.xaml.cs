using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
            InitializeFilters();
            LoadTasks();
            InitializeNotifications();
        }

        private void InitializeFilters()
        {
            try
            {
                // Загружаем приоритеты (общие для всех)
                var priorities = AppConnect.modelOdb.Priorities.ToList();
                priorities.Insert(0, new Priorities { PriorityID = 0, Name = "Все приоритеты" });
                PriorityFilter.ItemsSource = priorities;
                PriorityFilter.SelectedIndex = 0;

                // Загружаем статусы (общие для всех)
                var statuses = AppConnect.modelOdb.Statuses.ToList();
                statuses.Insert(0, new Statuses { StatusID = 0, Name = "Все статусы" });
                StatusFilter.ItemsSource = statuses;
                StatusFilter.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации фильтров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTasks()
        {
            try
            {
                var tasks = AppConnect.modelOdb.Tasks
                    .Include("Statuses")
                    .Include("Priorities")
                    .Where(t => t.UserID == AppConnect.CurrentUser.UserID)
                    .OrderByDescending(t => t.DueDate)
                    .ToList();

                TasksGrid.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке задач: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
            if(AppConnect.CurrentUser != null)
            {
                var dueTasks = AppConnect.modelOdb.Tasks
                .Where(t => t.UserID == AppConnect.CurrentUser.UserID &&
                           t.DueDate.HasValue &&
                           t.DueDate.Value <= deadline &&
                           t.StatusID != 3)
                .ToList();

                foreach (var task in dueTasks)
                {
                    NotificationExtenstions.ShowTaskDueNotification(task.Title, (DateTime)task.DueDate);
                }
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
            try
            {
                var query = AppConnect.modelOdb.Tasks
                    .Where(t => t.UserID == AppConnect.CurrentUser.UserID);

                // Фильтр по поиску
                if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                {
                    var searchText = SearchBox.Text.ToLower();
                    query = query.Where(t => t.Title.ToLower().Contains(searchText) ||
                                            t.Description.ToLower().Contains(searchText));
                }

                // Фильтр по приоритету
                if (PriorityFilter.SelectedValue is int priorityId && priorityId > 0)
                {
                    query = query.Where(t => t.PriorityID == priorityId);
                }

                // Фильтр по статусу
                if (StatusFilter.SelectedValue is int statusId && statusId > 0)
                {
                    query = query.Where(t => t.StatusID == statusId);
                }

                TasksGrid.ItemsSource = query.OrderByDescending(t => t.DueDate).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при применении фильтров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    "Вы уверены, что хотите удалить задачу?",
                    "Подтверждение удаления",
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
                        MessageBox.Show($"Ошибка при удалении задачи: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ViewStatistics_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new StatisticsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
            AppConnect.CurrentUser = null;
        }
    }
}