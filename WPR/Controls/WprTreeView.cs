using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using WPR.Infrastructure.Commands;
using WPR.Infrastructure.Extensions;

namespace WPR.Controls
{
    /// <summary>
    /// TreeView с управляемым выбором для MVVM.
    /// </summary>
    public class WprTreeView : TreeView
    {
        #region SelectedItem

        /// <summary>
        /// Выбранный элемент (TwoWay). Может становиться null.
        /// </summary>
        public new object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// DependencyProperty для <see cref="SelectedItem"/>.
        /// </summary>
        public new static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(WprTreeView),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tree = (WprTreeView)d;

            if (e.NewValue is null)
            {
                tree.TryClearSelectionInExpanded(tree);
                tree.CheckIsSelectionHidden();
                return;
            }

            if (tree.AutoExpandToSelected)
            {
                tree.Dispatcher.BeginInvoke(new Action(() =>
                {
                    tree.ExpandToItem(e.NewValue);
                    tree.TrySelectInExpandedBranches(e.NewValue);
                    tree.CheckIsSelectionHidden();
                }), DispatcherPriority.Background);

                return;
            }

            if (!tree.TrySelectInExpandedBranches(e.NewValue))
                tree.TryClearSelectionInExpanded(tree);
            tree.CheckIsSelectionHidden();
        }

        #endregion

        #region AutoExpandToSelected

        /// <summary>
        /// Если включено, при изменении <see cref="SelectedItem"/> дерево будет раскрывать ветки
        /// до выбранного элемента (если он найден в ItemsSource) и выделять его.
        /// </summary>
        public bool AutoExpandToSelected
        {
            get => (bool)GetValue(AutoExpandToSelectedProperty);
            set => SetValue(AutoExpandToSelectedProperty, value);
        }

        /// <summary>
        /// DependencyProperty для <see cref="AutoExpandToSelected"/>.
        /// </summary>
        public static readonly DependencyProperty AutoExpandToSelectedProperty =
            DependencyProperty.Register(
                nameof(AutoExpandToSelected),
                typeof(bool),
                typeof(WprTreeView),
                new FrameworkPropertyMetadata(false));

        #endregion

        #region readonly SelectedElementIsHidden : bool - Указывает, что выбранный элемент не null и скрыт в дереве

        /// <summary>Указывает, что выбранный элемент не null и скрыт в дереве</summary>
        private static readonly DependencyPropertyKey SelectedElementIsHiddenPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(SelectedElementIsHidden),
                typeof(bool),
                typeof(WprTreeView),
                new PropertyMetadata(false));

        /// <summary>Указывает, что выбранный элемент не null и скрыт в дереве</summary>
        public static readonly DependencyProperty SelectedElementIsHiddenProperty = SelectedElementIsHiddenPropertyKey.DependencyProperty;

        /// <summary>Указывает, что выбранный элемент не null и скрыт в дереве</summary>
        public bool SelectedElementIsHidden
        {
            get => (bool)GetValue(SelectedElementIsHiddenProperty);
            private set => SetValue(SelectedElementIsHiddenPropertyKey, value);
        }

        #endregion SelectedElementIsHidden : bool - Указывает, что выбранный элемент не null и скрыт в дереве

        #region RevealSelectedCommand

        /// <summary>
        /// Команда "Показать выбранный элемент".
        /// Раскрывает дерево до <see cref="SelectedItem"/> и пытается выделить/проскроллить его.
        /// </summary>
        public ICommand RevealSelectedCommand => field ??= new BaseCommand(RevealSelected);

        private void RevealSelected()
        {
            if (SelectedItem is null)
                return;

            Dispatcher.BeginInvoke(new Action(() =>
            {
                ExpandToItem(SelectedItem);
                TrySelectInExpandedBranches(SelectedItem);
            }), DispatcherPriority.Background);
        }

        #endregion


        /// <summary>
        /// Раскрывает дерево до указанного элемента.
        /// Если элемент не найден в ItemsSource, вернёт false.
        /// </summary>
        public void ExpandToItem(object item) => ExpandToItemCore(this, item, false);

        /// <summary>
        /// Раскрывает дерево с указанным элементом.
        /// Если элемент не найден в ItemsSource, вернёт false.
        /// </summary>
        public void ExpandItemContainer(object item) => ExpandToItemCore(this, item, true);

        /// <inheritdoc />
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Эти события нужны для восстановления выбора при раскрытии.
            AddHandler(TreeViewItem.ExpandedEvent, new RoutedEventHandler(OnAnyItemExpanded));

            // Ловим только клик по стрелке, так как остальные варианты всё равно выделят корень - и это нормально.
            // А вот клик по стрелке может сворачивать ветку с выбранным элементом внутри, и тогда нужно снять выделение заранее, чтобы оно не перешло на родителя.
            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown), true);
        }

        /// <inheritdoc />
        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);

            if (base.SelectedItem == null)
            {
                CheckIsSelectionHidden();
                return;
            }

            SelectedItem = e.NewValue;
            CheckIsSelectionHidden();
        }

        private void CheckIsSelectionHidden()
        {
            SelectedElementIsHidden = SelectedItem != null && base.SelectedItem == null;
        }

        private void OnAnyItemExpanded(object sender, RoutedEventArgs e)
        {
            // После раскрытия могли появиться контейнеры. Попробуем восстановить выбор.
            if (SelectedItem is not null)
                TrySelectInExpandedBranches(SelectedItem);
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Если клик по стрелочке/expander-у, и узел будет свёрнут -> снимем выделение заранее.
            if (e.OriginalSource is not DependencyObject origin)
                return;

            var toggle = origin.FindVisualParent<ToggleButton>();
            if (toggle is not { Name: "Expander" })
                return;

            var item = toggle.FindVisualParent<TreeViewItem>();
            if (item is null)
                return;

            // Если сейчас развёрнут, то клик приведёт к сворачиванию.
            if (item.IsExpanded)
                TryClearSelectionInExpanded(item);
        }


        // При сворачивании ветки снимаем выделение, но только если оно внутри сворачиваемого поддерева, и только в уже созданных контейнерах (то есть в видимых развёрнутых ветках).
        private void TryClearSelectionInExpanded(ItemsControl root)
        {
            foreach (var container in EnumerateContainers(root))
            {
                if (container.IsSelected)
                {
                    // Снимаем ТОЛЬКО визуальное выделение.
                    container.IsSelected = false;
                    return;
                }
            }
        }

        // При разворачивании ветки пытаемся восстановить выделение, но только если элемент уже видим в раскрытых ветках (то есть его контейнер уже создан).
        private bool TrySelectInExpandedBranches(object? item)
        {
            if (item is null)
                return false;

            var container = FindContainerInExpandedBranches(this, item);
            if (container is null)
                return false;

            container.IsSelected = true;
            container.BringIntoView();
            return true;
        }

        /// <summary>
        /// Обходит уже созданные TreeViewItem-контейнеры.
        /// В детей спускаемся только по развёрнутым веткам (это важно: в свёрнутых контейнеров детей обычно нет).
        /// </summary>
        private static IEnumerable<TreeViewItem> EnumerateContainers(ItemsControl parent)
        {
            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is not TreeViewItem container)
                    continue;

                yield return container;

                if (!container.IsExpanded)
                    continue;

                foreach (var child in EnumerateContainers(container))
                    yield return child;
            }
        }

        /// <summary>
        /// Ищет контейнер для item, спускаясь только по развёрнутым веткам.
        /// </summary>
        private static TreeViewItem? FindContainerInExpandedBranches(ItemsControl parent, object targetItem)
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(targetItem) is TreeViewItem direct)
                return direct;

            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is not TreeViewItem container)
                    continue;

                if (!container.IsExpanded)
                    continue;

                var found = FindContainerInExpandedBranches(container, targetItem);
                if (found is not null)
                    return found;
            }

            return null;
        }


        private static void ExpandToItemCore(ItemsControl parent, object targetItem, bool expandTarget)
        {
            // Если контейнер уже существует — отлично.
            if (parent.ItemContainerGenerator.ContainerFromItem(targetItem) is TreeViewItem direct)
            {
                if (expandTarget)
                    direct.IsExpanded = true;

                return;
            }

            foreach (var item in parent.Items)
            {
                if (parent.ItemContainerGenerator.ContainerFromItem(item) is not TreeViewItem container)
                    continue;

                if (ReferenceEquals(item, targetItem))
                {
                    if (expandTarget)
                        container.IsExpanded = true;

                    return;
                }

                if (!container.IsExpanded)
                    container.IsExpanded = true;

                // Продолжаем рекурсию уже после генерации дочерних контейнеров
                container.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ExpandToItemCore(container, targetItem, expandTarget);
                }), DispatcherPriority.Loaded);
            }
        }
    }
}
