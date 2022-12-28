using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace RetroSpy
{
    public partial class AddRemoveWindow : Window
    {
        private readonly AddRemoveWindowViewModel _vm;
        private readonly List<string>? _excludedList;
        readonly IReadOnlyList<InputSource>? _allSources;
        readonly Collection<string>? _originalExcludedList;
        
        public AddRemoveWindow(IReadOnlyList<InputSource> allSources, Collection<string> excludedList)
        {
            _vm = new AddRemoveWindowViewModel(this);
            DataContext = _vm;

            _originalExcludedList = excludedList;
            _excludedList = new List<string>(excludedList);
            _allSources = allSources;

            InitializeComponent();

            PopulateListBoxes();
        }

        private void PopulateListBoxes()
        {
            _vm.IncludedSources.Clear();
            _vm.ExcludedSources.Clear();
            
            if (_allSources == null)
                return;

            foreach (var source in _allSources)
            {
                if (_excludedList != null && _excludedList.Contains(source.Name))
                {
                    _vm.ExcludedSources.Add(source.Name);
                }
                else
                {
                    _vm.IncludedSources.Add(source.Name);
                }
            }
        }

        public AddRemoveWindow()
        {
            _vm = new AddRemoveWindowViewModel(this);
            DataContext = _vm;

            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_originalExcludedList != null && _excludedList != null)
            {
                _originalExcludedList.Clear();
                foreach (var source in _excludedList)
                {
                    _originalExcludedList.Add(source);
                }
            }
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (_excludedList != null && _vm.IncludedSources.GetSelectedId() != -1)
            {
                var selectedIndex = _vm.IncludedSources.GetSelectedId();
                if (_vm.IncludedSources.GetSelectedId() == _vm.IncludedSources.Count - 1 && _vm.IncludedSources.Count > 1)
                    selectedIndex -= 1;
                else if (_vm.IncludedSources.Count == 1)
                    selectedIndex = -1;

                _excludedList.Add(_vm.IncludedSources[_vm.IncludedSources.GetSelectedId()]);

                PopulateListBoxes();
                _vm.IncludedSources.SelectId(selectedIndex);

            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_excludedList != null && _vm.ExcludedSources.GetSelectedId() != -1)
            {
                var selectedIndex = _vm.ExcludedSources.GetSelectedId();

                if (_vm.ExcludedSources.GetSelectedId() == _vm.ExcludedSources.Count - 1 && _vm.ExcludedSources.Count > 1)
                    selectedIndex -= 1;
                else if (_vm.ExcludedSources.Count == 1)
                    selectedIndex = -1;

                _excludedList.Remove(_vm.ExcludedSources[_vm.ExcludedSources.GetSelectedId()]);

                PopulateListBoxes();
                _vm.ExcludedSources.SelectId(selectedIndex);

            }
        }

    }

    public class AddRemoveWindowViewModel
    {
        public ListView<string> IncludedSources { get; set; }
        public ListView<string> ExcludedSources { get; set; }
       
        public AddRemoveWindowViewModel(AddRemoveWindow addRemoveWindow)
        {
            IncludedSources = new ListView<string>();
            IncludedSources.StoreControl(addRemoveWindow, "IncludedListBox");
            ExcludedSources = new ListView<string>();
            ExcludedSources.StoreControl(addRemoveWindow, "ExcludedListBox");
        }

    }
}
