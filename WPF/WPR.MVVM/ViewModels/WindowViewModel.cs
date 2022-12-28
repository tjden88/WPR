namespace WPR.MVVM.ViewModels;

public abstract class WindowViewModel : ViewModel
{

    protected WindowViewModel(bool OnlyForDesignTime = false) : base(OnlyForDesignTime)
    {
    }

    #region Title : string - Заголовок окна

    /// <summary>Заголовок окна</summary>
    private string _Title = "Заголовок окна";

    /// <summary>Заголовок окна</summary>
    public string Title { get => _Title; set => Set(ref _Title, value); }

    #endregion
}