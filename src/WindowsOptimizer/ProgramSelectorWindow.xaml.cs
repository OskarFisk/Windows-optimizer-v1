using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace WindowsOptimizer;

public partial class ProgramSelectorWindow : Window
{
    private readonly ObservableCollection<ProgramEntry> _allPrograms = new();
    private readonly ObservableCollection<ProgramEntry> _visiblePrograms = new();

    private readonly string _selectionFile =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "WindowsOptimizer",
            "selected-programs.json");

    public ProgramSelectorWindow()
    {
        InitializeComponent();

        CategoryBox.Items.Add("All categories");
        CategoryBox.SelectedIndex = 0;

        LoadCatalog();
        ApplyFilter();
    }

    private void LoadCatalog()
    {
        try
        {
            var catalogPath = Path.Combine(
                AppContext.BaseDirectory,
                "software-master-list.txt");

            if (!File.Exists(catalogPath))
            {
                AddFallbackPrograms();
                return;
            }

            string? currentCategory = null;

            foreach (var rawLine in File.ReadLines(catalogPath))
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.StartsWith("#"))
                {
                    currentCategory = line.TrimStart('#').Trim();

                    if (!string.IsNullOrWhiteSpace(currentCategory) &&
                        !CategoryBox.Items.Contains(currentCategory))
                    {
                        CategoryBox.Items.Add(currentCategory);
                    }

                    continue;
                }

                if (currentCategory == null)
                    currentCategory = "Other";

                if (_allPrograms.Any(x =>
                    string.Equals(x.Name, line, StringComparison.OrdinalIgnoreCase)))
                    continue;

                _allPrograms.Add(new ProgramEntry
                {
                    Name = line,
                    Category = currentCategory
                });
            }
        }
        catch
        {
            AddFallbackPrograms();
        }
    }

    private void AddFallbackPrograms()
    {
        var fallback = new[]
        {
            ("Ninite", "Utilities"),
            ("MSI Afterburner", "Gaming & Overclocking"),
            ("AMD Ryzen Master", "Gaming & Overclocking"),
            ("BlueStacks", "Gaming"),
            ("OBS Studio", "Video & Streaming"),
            ("VLC", "Media")
        };

        foreach (var (name, category) in fallback)
        {
            if (!_allPrograms.Any(x => x.Name == name))
                _allPrograms.Add(new ProgramEntry
                {
                    Name = name,
                    Category = category
                });

            if (!CategoryBox.Items.Contains(category))
                CategoryBox.Items.Add(category);
        }
    }

    private void ApplyFilter()
    {
        var search = SearchBox.Text.Trim();
        var category = CategoryBox.SelectedItem?.ToString();

        _visiblePrograms.Clear();

        foreach (var program in _allPrograms)
        {
            var searchMatch =
                string.IsNullOrWhiteSpace(search) ||
                program.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                program.Category.Contains(search, StringComparison.OrdinalIgnoreCase);

            var categoryMatch =
                category == null ||
                category == "All categories" ||
                program.Category == category;

            if (searchMatch && categoryMatch)
                _visiblePrograms.Add(program);
        }

        ProgramsList.ItemsSource = _visiblePrograms;
        UpdateSelectionText();
    }

    private void SearchBox_TextChanged(object sender,
        System.Windows.Controls.TextChangedEventArgs e)
        => ApplyFilter();

    private void CategoryBox_SelectionChanged(object sender,
        System.Windows.Controls.SelectionChangedEventArgs e)
        => ApplyFilter();

    private void SelectVisible_Click(object sender, RoutedEventArgs e)
    {
        foreach (var program in _visiblePrograms)
            program.Selected = true;

        ProgramsList.Items.Refresh();
        UpdateSelectionText();
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        foreach (var program in _visiblePrograms)
            program.Selected = false;

        ProgramsList.Items.Refresh();
        UpdateSelectionText();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var directory = Path.GetDirectoryName(_selectionFile)!;
            Directory.CreateDirectory(directory);

            var selected = _allPrograms
                .Where(x => x.Selected)
                .Select(x => x.Name)
                .ToList();

            File.WriteAllText(
                _selectionFile,
                JsonSerializer.Serialize(selected,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

            MessageBox.Show(
                $"Saved {selected.Count} selected programs.",
                "Windows Optimizer",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Could not save selection",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
        => Close();

    private void UpdateSelectionText()
    {
        var count = _allPrograms.Count(x => x.Selected);
        SelectionText.Text =
            $"{count} selected • {_visiblePrograms.Count} programs shown";
    }
}

public sealed class ProgramEntry
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public bool Selected { get; set; }
}
