using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Parser.Models;
using Parser.Services;
using System.Globalization;

namespace Parser.ViewModels
{
    public class MainPageViewModel : BindableObject
    {
        private readonly IApiService _apiService;
        private DateTime _currentMonday;

        public ObservableCollection<Branch> Branches { get; } = new();
        public ObservableCollection<Year> Years { get; } = new();
        public ObservableCollection<Group> Groups { get; } = new();
        public ObservableCollection<DaySchedule> AllWeekSchedule { get; } = new();

        private Branch _selectedBranch;
        public Branch SelectedBranch
        {
            get => _selectedBranch;
            set { _selectedBranch = value; OnPropertyChanged(); if (value != null) LoadYearsCommand.Execute(null); }
        }

        private Year _selectedYear;
        public Year SelectedYear
        {
            get => _selectedYear;
            set { _selectedYear = value; OnPropertyChanged(); if (value != null) LoadGroupsCommand.Execute(null); }
        }

        private Group _selectedGroup;
        public Group SelectedGroup
        {
            get => _selectedGroup;
            set { _selectedGroup = value; OnPropertyChanged(); if (value != null) { _currentMonday = GetCurrentMonday(); LoadScheduleForWeekAsync(); } }
        }

        public ICommand LoadBranchesCommand { get; }
        public ICommand LoadYearsCommand { get; }
        public ICommand LoadGroupsCommand { get; }
        public ICommand LoadNextWeekScheduleCommand { get; }
        public ICommand LoadPreviousWeekScheduleCommand { get; }

        public MainPageViewModel(IApiService apiService)
        {
            _apiService = apiService;
            _currentMonday = GetCurrentMonday();
            LoadBranchesCommand = new Command(async () => await LoadBranchesAsync());
            LoadYearsCommand = new Command(async () => await LoadYearsAsync());
            LoadGroupsCommand = new Command(async () => await LoadGroupsAsync());
            LoadNextWeekScheduleCommand = new Command(async () => await LoadNextWeekScheduleAsync());
            LoadPreviousWeekScheduleCommand = new Command(async () => await LoadPreviousWeekScheduleAsync());
            LoadBranchesCommand.Execute(null);
        }

        private async Task LoadBranchesAsync()
        {
            var branches = await _apiService.GetBranchesAsync();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Branches.Clear();
                foreach (var b in branches) Branches.Add(b);
            });
        }

        private async Task LoadYearsAsync()
        {
            var years = await _apiService.GetYearsAsync();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Years.Clear();
                foreach (var y in years) Years.Add(y);
            });
        }

        private async Task LoadGroupsAsync()
        {
            if (SelectedBranch == null || SelectedYear == null) return;
            var groups = await _apiService.GetGroupsAsync(SelectedBranch.Id, SelectedYear.Id);
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Groups.Clear();
                foreach (var g in groups) Groups.Add(g);
            });
        }

        public async Task LoadNextWeekScheduleAsync()
        {
            _currentMonday = _currentMonday.AddDays(7);
            await LoadScheduleForWeekAsync();
        }

        public async Task LoadPreviousWeekScheduleAsync()
        {
            _currentMonday = _currentMonday.AddDays(-7);
            await LoadScheduleForWeekAsync();
        }

        private async Task LoadScheduleForWeekAsync()
        {
            if (SelectedBranch == null || SelectedGroup == null) return;
            var dateParam = _currentMonday.ToString("yyyy.MM.dd");
            var response = await _apiService.GetScheduleForGroupAsync(SelectedBranch.Id, SelectedGroup.Id, dateParam);
            if (response?.Schedule == null) return;
            var days = response.Schedule
                .Select(kvp => new DaySchedule
                {
                    Date = kvp.Key,
                    DayOfWeek = kvp.Key.Split('-').Last().Trim(),
                    Schedules = kvp.Value
                })
                .OrderBy(d => DateTime.ParseExact(d.Date.Split('-')[0].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture))
                .ToList();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                AllWeekSchedule.Clear();
                foreach (var day in days) AllWeekSchedule.Add(day);
            });
        }

        private DateTime GetCurrentMonday()
        {
            var today = DateTime.Now;
            var offset = (int)today.DayOfWeek - 1;
            return today.AddDays(-offset).Date;
        }
    }
}