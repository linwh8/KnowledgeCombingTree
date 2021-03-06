using Template10.Mvvm;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

namespace KnowledgeCombingTree.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private ObservableCollection<Models.TreeNode> searchedItems = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> SearchedItems { get { return this.searchedItems; } set { this.searchedItems = value; } }
        private ObservableCollection<Models.TreeNode> historyItems = new ObservableCollection<Models.TreeNode>();
        public ObservableCollection<Models.TreeNode> HistoryItems { get { return this.historyItems; } set { this.historyItems = value; } }
        private Models.TreeNode selectedItem = null;
        public Models.TreeNode SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }
        public MainPageViewModel()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                Value = "Designtime value";
            }
        }

        string _Value = "Gas";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            if (suspensionState.Any())
            {
                Value = suspensionState[nameof(Value)]?.ToString();
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        public void GotoDetailsPage() =>
            NavigationService.Navigate(typeof(Views.DetailPage), Value);

        public void GotoSettings() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 0);

        public void GotoPrivacy() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 1);

        public void GotoAbout() =>
            NavigationService.Navigate(typeof(Views.SettingsPage), 2);

    }
}

