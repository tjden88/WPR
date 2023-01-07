using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using WPR.MVVM.Commands.Base;

namespace WPR.MVVM.ViewModels;

/// <summary>
/// Базовая модель для редактирования сущности.
/// Может запоминать изначальные значения свойств и сбрасывать их.
/// Проверяет валидацию
/// </summary>
/// <typeparam name="TModel">Тип редактируемой сущности</typeparam>
public abstract class EditEntityViewModel<TModel> : ValidationViewModel
{
    /// <summary>
    /// Происходит при вызове команд подтверждения или отмены
    /// </summary>
    public event Action<bool> Completed; 


    // словарь для хранения изменённых свойств
    private readonly Dictionary<string, object> _Properties = new();


    /// <summary> Редактируемая сущность </summary>
    protected TModel Model { get; private set; }

    private readonly Type _ModelType;


    protected EditEntityViewModel(bool OnlyForDesignTime) : base(OnlyForDesignTime)
    {
        _ModelType = typeof(TModel);
    }

    protected EditEntityViewModel()
    {
        _ModelType = typeof(TModel);
    }

    protected EditEntityViewModel(TModel Model)
    {
        this.Model = Model;
        _ModelType = typeof(TModel);
    }


    /// <summary> Получить свойство вьюмодели </summary>
    protected virtual object GetValue([CallerMemberName] string Property = "") => _Properties
        .TryGetValue(Property, out var value) 
        ? value 
        : _ModelType.GetProperty(Property)?.GetValue(Model);



    /// <summary> Установить свойство вьюмодели </summary>
    protected virtual bool SetValue(object Value, [CallerMemberName] string Property = "")
    {
        if (_Properties.TryGetValue(Property, out var oldValue) && Equals(oldValue, Value))
            return false;

        var defaultValue = _ModelType.GetProperty(Property)?.GetValue(Model);

        if (Equals(defaultValue, Value))
            _Properties.Remove(Property);
        else
            _Properties[Property] = Value;

        OnPropertyChanged(Property);
        return true;
    }


    #region HeaderText : string - Заголовок

    /// <summary>Заголовок</summary>
    private string _HeaderText;

    /// <summary>Заголовок</summary>
    public string HeaderText
    {
        get => _HeaderText;
        set => Set(ref _HeaderText, value);
    }

    #endregion


    #region Commands

    #region Command CommitCommand - Подтвердить изменения и записать их в редактируемую модель 

    /// <summary>Подтвердить изменения и записать их в редактируемую модель </summary>
    private Command _CommitCommand;

    /// <summary>Подтвердить изменения и записать их в редактируемую модель </summary>
    public Command CommitCommand => _CommitCommand
        ??= new Command(OnCommitCommandExecuted, CanCommitCommandExecute, "Подтвердить изменения и записать их в редактируемую модель ");

    /// <summary>Проверка возможности выполнения - Подтвердить изменения и записать их в редактируемую модель </summary>
    private bool CanCommitCommandExecute() => !HasErrors && _Properties.Any();

    /// <summary>Логика выполнения - Подтвердить изменения и записать их в редактируемую модель </summary>
    private void OnCommitCommandExecuted()
    {
        OnCommit();
        ValidateAll();
        if(HasErrors) return;

        Completed?.Invoke(true);
    }

    #endregion


    #region Command CancelCommand - Отмена редактирования

    /// <summary>Отмена редактирования</summary>
    private Command _CancelCommand;

    /// <summary>Отмена редактирования</summary>
    public Command CancelCommand => _CancelCommand
        ??= new Command(OnCancelCommandExecuted, CanCancelCommandExecute, "Отмена редактирования");

    /// <summary>Проверка возможности выполнения - Отмена редактирования</summary>
    private bool CanCancelCommandExecute() => true;

    /// <summary>Логика выполнения - Отмена редактирования</summary>
    private void OnCancelCommandExecuted() => Completed?.Invoke(false);

    #endregion


    #region Command RejectCommand - Сбросить изменения

    /// <summary>Сбросить изменения</summary>
    private Command _RejectCommand;

    /// <summary>Сбросить изменения</summary>
    public Command RejectCommand => _RejectCommand
        ??= new Command(OnRejectCommandExecuted, CanRejectCommandExecute, "Сбросить изменения");

    /// <summary>Проверка возможности выполнения - Сбросить изменения</summary>
    private bool CanRejectCommandExecute() => _Properties.Any();

    /// <summary>Логика выполнения - Сбросить изменения</summary>
    private void OnRejectCommandExecuted() => OnReject();

    #endregion

    #endregion


    /// <summary> Применить изменения </summary>
    protected virtual void OnCommit()
    {
        foreach (var property in _Properties)
        {
            var modelProperty = _ModelType.GetProperty(property.Key);
            if (modelProperty is null || !modelProperty.CanWrite) continue;

            modelProperty.SetValue(Model, property.Value);
        }

        _Properties.Clear();
    }


    /// <summary> Сбросить изменения </summary>
    protected virtual void OnReject()
    {
        var changedProperties = _Properties.Keys.ToArray();
        _Properties.Clear();

        foreach (var property in changedProperties)
            OnPropertyChanged(property);
    }


    /// <summary> Установить новую модель </summary>
    public virtual void SetNewModel([NotNull] TModel model)
    {
        Model = model;
        _Properties.Clear();
        OnAllPropertiesChanged();
    }
}