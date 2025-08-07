using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WPR.Demo.Models;
using WPR.Dialogs;
using WPR.Mvvm.ViewModels;

namespace WPR.Demo;

/// <summary>
/// Логика взаимодействия для WprDialog.xaml
/// </summary>
public partial class WprDialog
{

    public WprDialog()
    {
        InitializeComponent();
    }
}


public partial class WprDialogViewModel : EditViewModel<Person>, IWPRDialog
{
    public event Action<bool> Completed;

    public object DialogContent { get; set; }

    [ObservableProperty]
    [Bind]
    private string _Name;

    [ObservableProperty]
    [Bind(nameof(Model.Age))]
    private int _Age;

    public WprDialogViewModel(Person model) : base(model)
    {
        CommandManager.RequerySuggested += (sender, args) => ResetCommand.NotifyCanExecuteChanged();
    }


    [RelayCommand]
    private void Save()
    {
       CommitChanges();
       Completed?.Invoke(true);
    }

    [RelayCommand]
    private void Cancel()
    {
        Completed?.Invoke(false);
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        ResetChanges();
    }

    public bool CanReset() => HasChanges;
}
