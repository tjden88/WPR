using CommunityToolkit.Mvvm.ComponentModel;

namespace WPR.Mvvm.Controls;

/// <summary>
/// Обёртка для TreeView: хранит исходный элемент и умеет отдавать детей.
/// Нужна для построения иерархии и биндинга выделения без TreeViewItem-магии.
/// </summary>
public class TreeViewModelWrapper<T>(T item, Func<T, IEnumerable<T>> getChildren, Action<T, bool>? onSelectedChanged = null) : ObservableObject where T : notnull
{
    private IReadOnlyList<TreeViewModelWrapper<T>>? _childrenCache;

    /// <summary>
    /// Исходный элемент, который будет отображён через ContentPresenter.
    /// </summary>
    public T Item { get; } = item;

    /// <summary>
    /// Дочерние узлы.
    /// ВАЖНО: кэшируем, иначе TreeView будет получать новые объекты на каждый доступ и всё развалится.
    /// </summary>
    public IEnumerable<TreeViewModelWrapper<T>> Children => _childrenCache ??= getChildren.Invoke(Item)
        .Select(o => new TreeViewModelWrapper<T>(o, getChildren, onSelectedChanged))
        .ToList();

    /// <summary>
    /// Сбрасывает кэш детей. Вызывается при пересборке дерева.
    /// </summary>
    public void RebuildChildren()
    {
        _childrenCache = null;
        OnPropertyChanged(nameof(Children));
    }

    /// <summary>
    /// Признак выделения узла.
    /// </summary>
    public bool IsSelected
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                onSelectedChanged?.Invoke(Item, value);
        }
    }

    /// <summary>
    /// Признак раскрытого узла.
    /// </summary>
    public bool IsExpanded
    {
        get;
        set => SetProperty(ref field, value);
    }
}