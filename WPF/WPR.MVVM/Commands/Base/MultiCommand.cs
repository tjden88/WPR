using System.Windows.Input;

namespace WPR.MVVM.Commands.Base;

/// <summary>
/// Последовательное выполнение двух команд
/// </summary>
public class MultiCommand : BaseCommand
{
    private readonly ICommand _First;
    private readonly ICommand _Second;

    /// <summary> Параметр первой команды </summary>
    public object FirstParameter { get; set; }

    /// <summary> Параметр второй команды </summary>
    public object SecondParameter { get; set; }


    public MultiCommand(ICommand First, ICommand Second)
    {
        _First = First;
        _Second = Second;
    }

    protected override void ExecuteCommand(object p)
    {
        _First.Execute(FirstParameter);
        _Second.Execute(SecondParameter);
    }
}