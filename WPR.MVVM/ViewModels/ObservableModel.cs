using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;

namespace WPR.Mvvm.ViewModels;

public abstract class ObservableModel<T>(T Model) : ObservableObject where T : class
{
    protected T Model { get; } = Model ?? throw new ArgumentNullException(nameof(Model));

    // преобразование VM -> Model
    public static implicit operator T(ObservableModel<T> vm)
    {
        if (vm == null) throw new ArgumentNullException(nameof(vm));
        vm.SetCollectionsToModel();
        return vm.Model;
    }

    
    
    #region SetCollections


    // Переносит данные из ObservableCollection-свойств VM назад в модель,
    // учитывая различные целевые типы коллекций в модели (List, Array, HashSet и пр.)
    private void SetCollectionsToModel()
    {
        var vmType = GetType();
        var modelType = typeof(T);

        foreach (var vmProp in vmType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            var vmPropType = vmProp.PropertyType;

            // Ищем только ObservableCollection<TItem>
            if (!vmPropType.IsGenericType || vmPropType.GetGenericTypeDefinition() != typeof(ObservableCollection<>))
                continue;

            var itemType = vmPropType.GetGenericArguments()[0];

            // Ищем одноимённое свойство в модели
            var modelProp = modelType.GetProperty(vmProp.Name, BindingFlags.Instance | BindingFlags.Public);
            if (modelProp == null) continue;

            var vmValue = vmProp.GetValue(this);
            if (vmValue == null)
            {
                // Если VM-коллекция null — стараемся либо обнулить свойство модели, либо очистить имеющуюся коллекцию
                if (modelProp.CanWrite)
                {
                    modelProp.SetValue(Model, null);
                }
                else
                {
                    var existing = modelProp.GetValue(Model);
                    TryClearCollection(existing);
                }
                continue;
            }

            // Приводим к IEnumerable<TItem> через Enumerable.Cast<TItem>
            var enumerableTyped = CastEnumerableTo(vmValue, itemType);

            // Пытаемся обновить существующую коллекцию (если у модели уже есть экземпляр и доступен get)
            if (TryUpdateExistingCollection(Model, modelProp, itemType, enumerableTyped))
                continue;

            // Иначе создаём новый экземпляр подходящего типа и присваиваем (если есть setter)
            var targetType = modelProp.PropertyType;
            var newValue = CreateCollectionForProperty(targetType, itemType, enumerableTyped);

            if (newValue != null && modelProp.CanWrite && targetType.IsInstanceOfType(newValue))
            {
                modelProp.SetValue(Model, newValue);
                continue;
            }

            // Если присвоить нельзя — попробуем хотя бы заменить содержимое существующей коллекции
            var existingCollection = modelProp.GetValue(Model);
            TryReplaceCollection(existingCollection, itemType, enumerableTyped);
        }
    }

    // Преобразование к IEnumerable<T> с помощью Enumerable.Cast<T>
    private static object CastEnumerableTo(object sourceEnumerable, Type itemType)
    {
        var castMethod = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == nameof(Enumerable.Cast) && m.GetParameters().Length == 1)
            .MakeGenericMethod(itemType);

        return castMethod.Invoke(null, new[] { sourceEnumerable })!;
    }

    // Создаёт новую коллекцию соответствующую типу свойства модели
    private static object? CreateCollectionForProperty(Type targetType, Type itemType, object enumerableTyped)
    {
        // Массивы
        if (targetType.IsArray && targetType.GetElementType() is { } elementType)
        {
            if (!elementType.IsAssignableFrom(itemType)) return null;
            return ToArray(enumerableTyped, itemType);
        }

        // Специфические известные типы
        if (IsGenericTypeDefinition(targetType, typeof(List<>)))
            return ToList(enumerableTyped, itemType);

        if (IsGenericTypeDefinition(targetType, typeof(HashSet<>)))
            return CreateWithIEnumerableCtor(typeof(HashSet<>).MakeGenericType(itemType), itemType, enumerableTyped);

        if (IsGenericTypeDefinition(targetType, typeof(ObservableCollection<>)))
            return CreateWithIEnumerableCtor(typeof(ObservableCollection<>).MakeGenericType(itemType), itemType, enumerableTyped);

        // Если целевой тип — один из интерфейсов коллекций, подставляем реализацию List<T>
        if (IsGenericInterface(targetType, typeof(ICollection<>)) ||
            IsGenericInterface(targetType, typeof(IList<>)) ||
            IsGenericInterface(targetType, typeof(IEnumerable<>)) ||
            IsGenericInterface(targetType, typeof(IReadOnlyCollection<>)))
        {
            var list = ToList(enumerableTyped, itemType);
            if (targetType.IsInstanceOfType(list)) return list; // редко, но может совпасть
            return list; // List<T> совместим с этими интерфейсами
        }

        // Если целевой тип совместим с List<T> — используем List<T>
        var listType = typeof(List<>).MakeGenericType(itemType);
        if (targetType.IsAssignableFrom(listType))
            return ToList(enumerableTyped, itemType);

        // Иначе пробуем найти конструктор targetType(IEnumerable<T>)
        var created = CreateWithIEnumerableCtor(targetType, itemType, enumerableTyped);
        if (created != null) return created;

        // Фолбэк — если targetType совместим с ObservableCollection<T>, попробуем его
        var ocType = typeof(ObservableCollection<>).MakeGenericType(itemType);
        if (targetType.IsAssignableFrom(ocType))
            return CreateWithIEnumerableCtor(ocType, itemType, enumerableTyped);

        return null;
    }

    // Пытается обновить существующую коллекцию модели (Clear/Add)
    private static bool TryUpdateExistingCollection(object modelInstance, PropertyInfo modelProp, Type itemType, object enumerableTyped)
    {
        if (!modelProp.CanRead) return false;

        var existing = modelProp.GetValue(modelInstance);
        if (existing == null) return false;

        // Отдельно обработаем массивы: заменить нельзя — потребуется создать новый
        if (existing.GetType().IsArray)
            return false;

        // Сначала пробуем generic ICollection<T>
        var iCollectionOfT = typeof(ICollection<>).MakeGenericType(itemType);
        if (iCollectionOfT.IsInstanceOfType(existing))
        {
            var collType = existing.GetType();
            var clear = collType.GetMethod("Clear");
            var add = collType.GetMethod("Add", [itemType]);
            
            if(clear is null || add is null)
                return false;

            clear?.Invoke(existing, null);

            foreach (var item in (IEnumerable)enumerableTyped)
                add?.Invoke(existing, new[] { item });

            return true;
        }

        // Далее — негeneric IList
        if (existing is IList nonGenericList)
        {
            nonGenericList.Clear();
            foreach (var item in (IEnumerable)enumerableTyped)
                nonGenericList.Add(item);
            return true;
        }
        return false;
    }

    // Если присвоить нельзя, но есть уже коллекция — пробуем очистить и наполнить
    private static void TryReplaceCollection(object? existing, Type itemType, object enumerableTyped)
    {
        if (existing == null) return;

        var iCollectionOfT = typeof(ICollection<>).MakeGenericType(itemType);
        if (iCollectionOfT.IsInstanceOfType(existing))
        {
            var collType = existing.GetType();
            var clear = collType.GetMethod("Clear");
            var add = collType.GetMethod("Add", new[] { itemType });

            clear?.Invoke(existing, null);
            foreach (var item in (IEnumerable)enumerableTyped)
                add?.Invoke(existing, new[] { item });
            return;
        }

        if (existing is IList nonGenericList)
        {
            nonGenericList.Clear();
            foreach (var item in (IEnumerable)enumerableTyped)
                nonGenericList.Add(item);
        }
    }

    private static void TryClearCollection(object? existing)
    {
        if (existing == null) return;

        if (existing is IList nonGenericList)
        {
            nonGenericList.Clear();
            return;
        }

        var iCollection = existing.GetType().GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));

        if (iCollection != null)
        {
            var clear = existing.GetType().GetMethod("Clear");
            clear?.Invoke(existing, null);
        }
    }

    // Вызовы Linq-методов для конкретного T
    private static object ToArray(object enumerableTyped, Type itemType)
    {
        var toArray = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == nameof(Enumerable.ToArray) && m.GetParameters().Length == 1)
            .MakeGenericMethod(itemType);
        return toArray.Invoke(null, new[] { enumerableTyped })!;
    }

    private static object ToList(object enumerableTyped, Type itemType)
    {
        var toList = typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == nameof(Enumerable.ToList) && m.GetParameters().Length == 1)
            .MakeGenericMethod(itemType);
        return toList.Invoke(null, new[] { enumerableTyped })!;
    }

    private static object? CreateWithIEnumerableCtor(Type targetType, Type itemType, object enumerableTyped)
    {
        var ienumerableOfT = typeof(IEnumerable<>).MakeGenericType(itemType);
        var ctor = targetType.GetConstructor(new[] { ienumerableOfT });
        if (ctor == null) return null;
        return ctor.Invoke(new[] { enumerableTyped });
    }

    private static bool IsGenericTypeDefinition(Type type, Type genericDef)
        => type.IsGenericType && type.GetGenericTypeDefinition() == genericDef;

    private static bool IsGenericInterface(Type type, Type genericDef)
        => type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == genericDef;
    

    #endregion
}