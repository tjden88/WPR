using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace WPR.Mvvm.Controls;

/// <summary>
/// Расширенная коллекция с поддержкой массового добавления, удаления и замены элементов с минимальным количеством событий.
/// </summary>
/// <typeparam name="T">Тип элементов коллекции.</typeparam>
public class ObservableCollectionEx<T> : ObservableCollection<T>
{
    // Флаг, блокирующий уведомления, чтобы не спамить событиями при массовых операциях
    private bool _suppressNotification = false;

    /// <summary>
    /// Добавляет сразу несколько элементов в коллекцию, вызвав одно событие обновления.
    /// </summary>
    /// <param name="items">Коллекция добавляемых элементов.</param>
    /// <exception cref="ArgumentNullException">Если items равен null.</exception>
    public void AddRange(IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        _suppressNotification = true;

        try
        {
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }
        finally
        {
            _suppressNotification = false;
            // Одно событие: сброс коллекции, чтобы UI обновился корректно
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
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

        _suppressNotification = true;

        try
        {
            foreach (var item in items)
            {
                Items.Remove(item);
            }
        }
        finally
        {
            _suppressNotification = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }
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

        _suppressNotification = true;

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
            _suppressNotification = false;
            // Уведомляем UI, что коллекция полностью обновлена
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        }
    }

    /// <summary>
    /// Переопределение события изменения коллекции с учётом подавления уведомлений.
    /// </summary>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressNotification)
            base.OnCollectionChanged(e);
    }

    /// <summary>
    /// Переопределение события изменения свойств с учётом подавления уведомлений.
    /// </summary>
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (!_suppressNotification)
            base.OnPropertyChanged(e);
    }
    
}