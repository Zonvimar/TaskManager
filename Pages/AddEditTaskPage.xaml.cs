using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskManager.ApplicationData;

namespace TaskManager.Pages
{
    public partial class AddEditTaskPage : Page
    {
        private Tasks _currentTask;
        private bool _isEditMode;

        public AddEditTaskPage(Tasks task = null)
        {
            InitializeComponent();
            InitializePage(task);
            LoadComboBoxes();
        }

        private void InitializePage(Tasks task)
        {
            if (task != null)
            {
                _currentTask = task;
                LoadTaskData();
                _isEditMode = true;
                PageTitle.Text = "Редактировать задачу";
            }
            else
            {
                _currentTask = new Tasks();
                _isEditMode = false;
                PageTitle.Text = "Новая задача";
                SetDefaultValues();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                // Загружаем приоритеты (общие для всех)
                var priorities = AppConnect.modelOdb.Priorities.ToList();
                PriorityComboBox.ItemsSource = priorities;

                // Загружаем статусы (общие для всех)
                var statuses = AppConnect.modelOdb.Statuses.ToList();
                StatusComboBox.ItemsSource = statuses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке приоритетов/статусов: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadTaskData()
        {
            TitleTextBox.Text = _currentTask.Title;
            DescriptionTextBox.Text = _currentTask.Description;

            if (_currentTask.DueDate.HasValue)
            {
                DueDatePicker.SelectedDate = _currentTask.DueDate.Value.Date;
                TimeHourTextBox.Text = _currentTask.DueDate.Value.Hour.ToString("D2");
                TimeMinuteTextBox.Text = _currentTask.DueDate.Value.Minute.ToString("D2");
            }

            PriorityComboBox.SelectedValue = _currentTask.PriorityID;
            StatusComboBox.SelectedValue = _currentTask.StatusID;
        }

        private void SetDefaultValues()
        {
            DueDatePicker.SelectedDate = DateTime.Today;
            TimeHourTextBox.Text = DateTime.Now.Hour.ToString("D2");
            TimeMinuteTextBox.Text = DateTime.Now.Minute.ToString("D2");

            // Устанавливаем значения по умолчанию
            var defaultPriority = AppConnect.modelOdb.Priorities
                .FirstOrDefault(p => p.Name == "Низкий");
            if (defaultPriority != null)
                PriorityComboBox.SelectedValue = defaultPriority.PriorityID;

            var defaultStatus = AppConnect.modelOdb.Statuses
                .FirstOrDefault(s => s.Name == "К выполнению");
            if (defaultStatus != null)
                StatusComboBox.SelectedValue = defaultStatus.StatusID;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateInput())
                {
                    UpdateTaskData();

                    if (_isEditMode)
                    {
                        AppConnect.modelOdb.SaveChanges();
                    }
                    else
                    {
                        AppConnect.modelOdb.Tasks.Add(_currentTask);
                        AppConnect.modelOdb.SaveChanges();
                    }

                    NavigationService.Navigate(new TasksPage());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении задачи: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название задачи.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!DueDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите срок выполнения.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TimeHourTextBox.Text, out int hour) || hour < 0 || hour > 23)
            {
                MessageBox.Show("Введите корректный час (0-23).", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TimeMinuteTextBox.Text, out int minute) || minute < 0 || minute > 59)
            {
                MessageBox.Show("Введите корректные минуты (0-59).", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (PriorityComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите приоритет.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (StatusComboBox.SelectedValue == null)
            {
                MessageBox.Show("Выберите статус.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void UpdateTaskData()
        {
            _currentTask.Title = TitleTextBox.Text;
            _currentTask.Description = DescriptionTextBox.Text;
            _currentTask.DueDate = DueDatePicker.SelectedDate.Value
                .AddHours(int.Parse(TimeHourTextBox.Text))
                .AddMinutes(int.Parse(TimeMinuteTextBox.Text));
            _currentTask.PriorityID = (int)PriorityComboBox.SelectedValue;
            _currentTask.StatusID = (int)StatusComboBox.SelectedValue;

            if (!_isEditMode)
            {
                _currentTask.UserID = AppConnect.CurrentUser.UserID;
                _currentTask.CreatedDate = DateTime.Now;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TasksPage());
        }
    }
}