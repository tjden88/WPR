namespace WPR.MVVM.ViewModels;

partial class ViewModel
{
    public readonly ref struct ValueResult<T>
    {
        private readonly bool _Result;
        private readonly T _NewValue;
        private readonly T _OldValue;
        private readonly ViewModel _Model;

        public ValueResult(bool Result, in T NewValue, in T OldValue, ViewModel Model)
        {
            _Result = Result;
            _NewValue = NewValue;
            _OldValue = OldValue;
            _Model = Model;
        }

        public ValueResult<T> Then(Action Action)
        {
            if (_Result) Action();
            return this;
        }

        public ValueResult<T> Then(Action<T> Action)
        {
            if (_Result) Action(_NewValue);
            return this;
        }

        public ValueResult<T> ThenOld(Action<T> Action)
        {
            if (_Result) Action(_OldValue);
            return this;
        }

        public ValueResult<T> ThenIf(Predicate<T> ConditionPredicate, Action Action)
        {
            if (_Result && ConditionPredicate(_NewValue))
                Action();
            return this;
        }

        public ValueResult<T> ThenIf(Predicate<T> ConditionPredicate, Action<T> Action)
        {
            if (_Result && ConditionPredicate(_NewValue))
                Action(_NewValue);
            return this;
        }

        public ValueResult<T> ThenIfOld(Predicate<T> ConditionPredicate, Action<T> Action)
        {
            if (_Result && ConditionPredicate(_OldValue))
                Action(_OldValue);
            return this;
        }

        public ValueResult<T> CallPropertyChanged(string PropertyName)
        {
            if (_Result) _Model.OnPropertyChanged(PropertyName);
            return this;
        }

        public ValueResult<T> CallAllPropertiesChanged()
        {
            if (_Result) _Model.OnAllPropertiesChanged();
            return this;
        }
    }
}