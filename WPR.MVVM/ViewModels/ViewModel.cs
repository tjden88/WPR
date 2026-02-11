using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Reflection;
using WPR.Mvvm.Attributes;

namespace WPR.Mvvm.ViewModels;

/// <summary>
/// Базовая модель-представление для MVVM. Реализует <see cref="ObservableObject"/>
/// </summary>
public abstract class ViewModel : ObservableObject
{
    private readonly HashSet<string> _raiseAfterPropertyChanged = new(StringComparer.Ordinal);

    /// <summary>
    /// Отключает вызов AfterPropertyChanged для всех свойств, даже помеченных атрибутом RaiseAfterPropertyChangedAttribute.
    /// По умолчанию выключено, то есть AfterPropertyChanged НЕ будет вызываться для свойств с атрибутом RaiseAfterPropertyChangedAttribute.
    /// Рекомендуется включать только после инициализации модели-представления, чтобы избежать лишних вызовов AfterPropertyChanged во время конструктора инициализации.
    /// </summary>
    protected bool EnableAfterPropertyChangedInvocation { get; set; }

    protected ViewModel()
    {
        InitializeAttributes();
        this.InitializeAutoNotifyCanExecuteChangedAttribute();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if(!EnableAfterPropertyChangedInvocation)
            return;

        var propertyName = e.PropertyName;

        // Если имя свойства не задано (null/empty) - это сигнал "изменилось всё".
        // В этом случае пропускать AfterPropertyChanged по атрибутам бессмысленно.
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            AfterPropertyChanged(e);
            return;
        }

        // Если свойство помечено RaiseAfterPropertyChangedAttribute - вызываем AfterPropertyChanged.
        if (_raiseAfterPropertyChanged.Contains(propertyName))
            AfterPropertyChanged(e);
    }

    /// <summary>
    /// Вызывается после OnPropertyChanged, только в том случае, если свойство, которое инициировало OnPropertyChanged
    /// помечено атрибутом RaiseAfterPropertyChangedAttribute.
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
            if (p.GetCustomAttribute<RaiseAfterPropertyChangedAttribute>(inherit: true) is not null)
                _raiseAfterPropertyChanged.Add(p.Name);
        }
    }

    #endregion
}

