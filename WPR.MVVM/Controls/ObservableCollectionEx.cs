using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace WPR.Mvvm.Controls;

/// <summary>
/// Расширенная ObservableCollection с поддержкой массовых операций 
/// и автоматической маршализацией событий в UI-поток
/// </summary>
public class ObservableCollectionEx<T> : ObservableCollection<T>
{
    public ObservableCollectionEx() { }

    public ObservableCollectionEx(IEnumerable<T> items) : base(items) { }
    public ObservableCollectionEx(List<T> items) : base(items) { }


    // Флаг, блокирующий уведомления, чтобы не спамить событиями при массовых операциях
    private bool _SuppressNotification = false;

    /// <summary> Проверяет, нужно ли маршализовать вызов в UI-поток </summary>
    private static bool NeedsInvoke => Application.Current?.Dispatcher != null &&
                                       !Application.Current.Dispatcher.CheckAccess();

    /// <summary>
    /// Добавляет сразу несколько элементов в коллекцию, вызвав одно событие обновления.
    /// </summary>
    /// <param name="items">Коллекция добавляемых элементов.</param>
    /// <exception cref="ArgumentNullException">Если items равен null.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        _SuppressNotification = true;

        try
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
        finally
        {
            _SuppressNotification = false;
            // Одно событие: сброс коллекции, чтобы UI обновился корректно
            NotifyReset();
        }
    }

    /// <summary>
    /// Удаляет несколько элементов из коллекции с одним событием обновления.
    /// </summary>
    /// <param name="items">Коллекция удаляемых элементов.</param>
    /// <exception cref="ArgumentNullException">Если items равен null.</exception>
    public void RemoveRange(IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        _SuppressNotification = true;

        try
        {
            foreach (var item in items)
            {
                Items.Remove(item);
            }
        }
        finally
        {
            _SuppressNotification = false;
            NotifyReset();
        }
    }
    
    /// <summary>
    /// Заменяет элемент <paramref name="oldItem"/> на <paramref name="newItem"/> в коллекции, сохраняя позицию.
    /// Если oldItem не найден — ничего не делает.
    /// Генерирует корректные события коллекции.
    /// </summary>
    public bool Replace(T oldItem, T newItem)
    {
        var index = IndexOf(oldItem);
        if (index < 0)
            return false; // не нашли

        SetItem(index, newItem);
        return true;
    }

    /// <summary>
    /// Заменяет все элементы коллекции на новые, вызвав одно событие обновления.
    /// </summary>
    /// <param name="newItems">Новая коллекция элементов.</param>
    /// <exception cref="ArgumentNullException">Если newItems равен null.</exception>
    public void ReplaceAll(IEnumerable<T> newItems)
    {
        if (newItems == null)
            throw new ArgumentNullException(nameof(newItems));

        _SuppressNotification = true;

        try
        {
            Items.Clear();

            foreach (var item in newItems)
            {
                Items.Add(item);
            }
        }
        finally
        {
            _SuppressNotification = false;
            // Уведомляем UI, что коллекция полностью обновлена
            NotifyReset();
        }
    }

    /// <summary>
    /// Переопределение события изменения коллекции с учётом подавления уведомлений.
    /// </summary>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (_SuppressNotification) return;

        if (NeedsInvoke)
        {
            Application.Current.Dispatcher.InvokeAsync(() => base.OnCollectionChanged(e));
        }
        else
        {
            base.OnCollectionChanged(e);
        }
    }

    /// <summary>
    /// Переопределение события изменения свойств с учётом подавления уведомлений.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (_SuppressNotification) return;

        if (NeedsInvoke)
        {
            Application.Current.Dispatcher.InvokeAsync(() => base.OnPropertyChanged(e));
        }
        else
        {
            base.OnPropertyChanged(e);
        }
    }

    private void NotifyReset()
    {
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
    }
}