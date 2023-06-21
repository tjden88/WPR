using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;

namespace WPR.MVVM.Commands.Base;

public abstract class BaseCommand : MarkupExtension, ICommand, INotifyPropertyChanged
{

    #region Executable: bool

    /// <summary>Происходит при изменении ручной возможности исполнения команды</summary>
    public event EventHandler<bool> ExecutableChanged;

    private bool _Executable = true;

    /// <summary>Ручная возможность выполнения команды</summary>
    public bool Executable
    {
        get => _Executable;
        set
        {
            if (_Executable == value) return;
            _Executable = value;
            CommandManager.InvalidateRequerySuggested();
            ExecutableChanged?.Invoke(this, value);
            OnPropertyChanged();
        }
    }
    #endregion


    #region Visibility : Visibility - Видимость, если команда доступна

    /// <summary>Видимость, если команда доступна</summary>
    private Visibility _Visibility = Visibility.Collapsed;

    /// <summary>Видимость, если команда доступна</summary>
    public Visibility Visibility
    {
        get => _Visibility;
        set
        {
            if(Equals(_Visibility, value)) return;
            _Visibility = value;
            OnPropertyChanged();
        }
    }

    #endregion


    #region Текст команды
    private string _CommandText;
    /// <summary> Текст - описание команды </summary>
    public string Text
    {
        get
        {
            var gString = ExecuteGesture?.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
            return gString == null ? _CommandText : $"{_CommandText} ({gString})";
        }
        set
        {
            if (value == _CommandText) return;
            _CommandText = value;
            OnPropertyChanged();
        }
    }

    #endregion


    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

    #endregion


    /// <summary> Комбинация клавиш быстрого доступа </summary>
    protected KeyGesture ExecuteGesture { get; init; }

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>Возможность выполнения команды</summary>
    public bool CanExecute(object parameter)
    {
        var canExecute = _Executable && CanExecuteCommand(parameter);
        Visibility = canExecute ? Visibility.Visible : Visibility.Collapsed;
        return canExecute;
    }

    /// <summary>Выполнить команду с параметром</summary>
    public void Execute(object parameter)
    {
        if (!CanExecute(parameter)) return;
        ExecuteCommand(parameter);
    }

    ///// <summary> Выполнить команду без параметра </summary>
    public virtual void Execute() => Execute(null);

    /// <summary>Возможность выполнения команды</summary>
    protected virtual bool CanExecuteCommand(object p) => true;

    /// <summary>Действие выполнения команды</summary>
    protected abstract void ExecuteCommand(object p);

    public override string ToString() => Text;


    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}



public abstract class BaseCommand<T> : BaseCommand
{
    #region CanExecuteWithNullParameter

    private bool _CanExecuteWithNullParameter;

    /// <summary>Разрешить выполнение команды с параметром = null</summary>
    public bool CanExecuteWithNullParameter
    {
        get => _CanExecuteWithNullParameter;
        set
        {
            if (_CanExecuteWithNullParameter == value) return;
            _CanExecuteWithNullParameter = value;
            CommandManager.InvalidateRequerySuggested();
            OnPropertyChanged();
        }
    }

    #endregion

    /// <summary>Возможность выполнения команды</summary>
    protected override bool CanExecuteCommand(object P)
    {

        if (!CanExecuteWithNullParameter && P is not T) return false;
        return true;
    }

    protected sealed override void ExecuteCommand(object p) => ExecuteCommand((T)p);

    /// <summary>Выполнить команду с параметорм типа</summary>
    public abstract void ExecuteCommand(T p);
}