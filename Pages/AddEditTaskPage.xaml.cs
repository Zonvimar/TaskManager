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
    /// <summary>
    /// Логика взаимодействия для AddEditTaskPage.xaml
    /// </summary>
    public partial class AddEditTaskPage : Page
    {
        private Tasks _currentTask;
        private bool _isEditMode;

        public AddEditTaskPage(Tasks task = null)
        {
            InitializeComponent();
            InitializePage(task);
        }

        private void InitializePage(Tasks task)
        {
            if (task != null)
            {
                _currentTask = task;
                _isEditMode = true;
                PageTitle.Text = "Редактирование задачи";
                LoadTaskData();
            }
            else
            {
                _currentTask = new Tasks();
                _isEditMode = false;
                PageTitle.Text = "Добавление задачи";
                SetDefaultValues();
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

            PriorityComboBox.SelectedIndex = (int)(_currentTask.Priority - 1);
            StatusComboBox.SelectedIndex = (int)(_currentTask.Status - 1);
        }

        private void SetDefaultValues()
        {
            DueDatePicker.SelectedDate = DateTime.Today;
            TimeHourTextBox.Text = DateTime.Now.Hour.ToString("D2");
            TimeMinuteTextBox.Text = DateTime.Now.Minute.ToString("D2");
            PriorityComboBox.SelectedIndex = 0;
            StatusComboBox.SelectedIndex = 0;
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
                MessageBox.Show($"Ошибка сохранения задачи: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Пожалуйста введите название задачи.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!DueDatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Пожалуйста выберите дадту дедлайна.", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TimeHourTextBox.Text, out int hour) || hour < 0 || hour > 23)
            {
                MessageBox.Show("Пожалуйста, введите час корректно (0-23).", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (!int.TryParse(TimeMinuteTextBox.Text, out int minute) || minute < 0 || minute > 59)
            {
                MessageBox.Show("Пожалуйста, введите минуты корректно (0-59).", "Ошибка валидации",
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
            _currentTask.Priority = PriorityComboBox.SelectedIndex + 1;
            _currentTask.Status = StatusComboBox.SelectedIndex + 1;

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
