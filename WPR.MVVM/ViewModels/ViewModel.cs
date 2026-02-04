using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Reflection;
using WPR.Mvvm.Attributes;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM. Реализует <see cref="ObservableObject"/>
/// </summary>
public abstract class ViewModel : ObservableObject
{
    private readonly HashSet<string> _skipAfterPropertyChanged = new(StringComparer.Ordinal);

    protected ViewModel()
    {
        InitializeAttributes();
        this.InitializeAutoNotifyCanExecuteChangedAttribute();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        // Если имя свойства не задано (null/empty) - это сигнал "изменилось всё".
        // В этом случае пропускать AfterPropertyChanged по атрибутам бессмысленно.
        var propertyName = e.PropertyName;
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            AfterPropertyChanged(e);
            return;
        }

        // Если свойство помечено SkipAfterPropertyChangedAttribute - AfterPropertyChanged не вызываем.
        if (_skipAfterPropertyChanged.Contains(propertyName))
            return;

        AfterPropertyChanged(e);
    }

    /// <summary>
    /// Вызывается после OnPropertyChanged, только в том случае, если свойство, которое инициировало OnPropertyChanged
    /// не помечено атрибутом SkipAfterPropertyChangedAttribute.
    /// </summary>
    protected virtual void AfterPropertyChanged(PropertyChangedEventArgs e) { }


    #region InitializeAttributes

    /// <summary>
    /// Инициализирует кэш атрибутов модели-представления.
    /// </summary>
    private void InitializeAttributes()
    {
        // Собираем свойства, для которых AfterPropertyChanged не должен вызываться.
        // Кэш строим один раз на экземпляр VM. Этого достаточно и просто.
        var type = GetType();
        var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var p in props)
        {
            if (p.GetCustomAttribute<SkipAfterPropertyChangedAttribute>(inherit: true) is not null)
                _skipAfterPropertyChanged.Add(p.Name);
        }
    }

    #endregion
}

