using WPR.Domain.Interfaces;
using WPR.Domain.Models.Themes;

namespace WPR.UiServices.UI;

/// <summary>
/// Помощник рутинных операций
/// </summary>
public class ActionHelper
{

    public enum MessageType
    {
        Default,
        Danger,
        Success
    }

    private readonly IUserDialog _UserDialog;

    public ActionHelper(IUserDialog UserDialog) =>
        _UserDialog = UserDialog;


    private bool _SuccessResult;
    private readonly List<IActionHelperTask> _Tasks = new();// Очередь задач


    #region AddActions

    /// <summary>
    /// Добавить синхронную задачу в очередь выполнения
    /// </summary>
    /// <param name="action">Задача на выполнение</param>
    public ActionHelperTask AddAction(Action action) => AddAction(action, () => true);


    /// <summary>
    /// Добавить синхронную задачу в очередь выполнения
    /// </summary>
    /// <param name="action">Задача на выполнение</param>
    /// <param name="CheckSuccess">Проверка на успех</param>
    /// <param name="OnFailMessage">Сообщение пользователю при неудаче</param>

    public ActionHelperTask AddAction(Action action, Func<bool> CheckSuccess, string? OnFailMessage = null)
    {
        var task = new ActionHelperTask(this, () =>
            {
                action.Invoke();
                return Task.FromResult(CheckSuccess());
            },
            OnFailMessage);

        return Add(task);
    }

    /// <summary>
    /// Добавить синхронную задачу в очередь выполнения
    /// </summary>
    /// <typeparam name="T">Тип, возвращаемый задачей</typeparam>
    /// <param name="action">Задача на выполнение</param>
    /// <param name="CheckSuccess">Проверка на успех</param>
    /// <param name="OnFailMessage">Сообщение пользователю при неудаче</param>
    /// <returns></returns>
    public ActionHelperTask<T> AddAction<T>(Func<T> action, Predicate<T> CheckSuccess, string? OnFailMessage = null)
    {
        var task = new ActionHelperTask<T>(this, () =>
            {
                var result = action.Invoke();
                return Task.FromResult(new ActionHelperTask<T>.ActionResult(CheckSuccess(result), result));
            }, OnFailMessage);

        return Add(task);
    }

    #endregion

    #region AddTasks

    /// <summary>
    /// Добавить задачу в очередь выполнения
    /// </summary>
    /// <param name="action">Задача на выполнение</param>
    public ActionHelperTask AddTask(Func<Task> action) => AddTask(action, () => true);


    /// <summary>
    /// Добавить задачу в очередь выполнения
    /// </summary>
    /// <param name="action">Задача на выполнение</param>
    /// <param name="CheckSuccess">Проверка на успех</param>
    /// <param name="OnFailMessage">Сообщение пользователю при неудаче</param>

    public ActionHelperTask AddTask(Func<Task> action, Func<bool> CheckSuccess, string? OnFailMessage = null)
    {
        var task = new ActionHelperTask(this,
            async () =>
            {
                await action();
                return CheckSuccess();
            },
            OnFailMessage);

        return Add(task);
    }


    /// <summary>
    /// Добавить задачу в очередь выполнения
    /// </summary>
    /// <typeparam name="T">Тип, возвращаемый задачей</typeparam>
    /// <param name="action">Задача на выполнение</param>
    /// <param name="CheckSuccess">Проверка на успех</param>
    /// <param name="OnFailMessage">Сообщение пользователю при неудаче</param>
    /// <returns></returns>
    public ActionHelperTask<T> AddTask<T>(Func<Task<T>> action, Predicate<T> CheckSuccess, string? OnFailMessage = null)
    {

        var task = new ActionHelperTask<T>(this,
            async () =>
            {
                var result = await action();
                var checkSuccess = CheckSuccess(result);
                return new ActionHelperTask<T>.ActionResult(checkSuccess, result);
            }, OnFailMessage);

        return Add(task);
    }

    #endregion

    #region Dialogs


    /// <summary>
    /// Вопрос пользователю
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="Title">Заголовок</param>
    /// <param name="OnCancelMessage">Сообщение пользователю при отказе</param>
    public ActionHelperTask Question(string message, string? Title = "Внимание", string? OnCancelMessage = null)
    {
        var task = new ActionHelperTask(this,
            async () =>
            {
                var result = await _UserDialog.QuestionAsync(message, Title);
                return result;
            }, OnCancelMessage);

        return Add(task);

    }


    /// <summary>
    /// Всплывающее уведомление
    /// </summary>
    /// <param name="message">Сообщение</param>
    /// <param name="MessageType">Тип сообщения</param>
    public ActionHelperTask Notification(string message, MessageType MessageType = MessageType.Default)
    {
        var color = MessageType switch
        {
            MessageType.Default => StyleBrushes.BackgroundContrastColorBrush,
            MessageType.Danger => StyleBrushes.DangerColorBrush,
            MessageType.Success => StyleBrushes.SuccessColorBrush,
            _ => StyleBrushes.BackgroundContrastColorBrush,
        };


        var task = new ActionHelperTask(this,
            async () =>
            {
                await _UserDialog.ShowNotificationAsync(message, Backgound: color);
                return true;
            }, null);

        return Add(task);

    }

    #endregion


    // Добавить в список
    private T Add<T>(T task) where T : IActionHelperTask
    {
        _Tasks.Add(task);
        return task;
    }


    /// <summary>
    /// Начать выполнение задач в очереди
    /// </summary>
    /// <param name="BreakOnFail">Прервать выполнение при неудачной проверке выполнения</param>
    /// <returns>True, если все задачи выполнены успешно</returns>
    public async Task<bool> ExecuteAsync(bool BreakOnFail = true)
    {
        if (!_Tasks.Any())
            throw new InvalidOperationException("Список задач пуст!");

        _SuccessResult = true;
        foreach (var task in _Tasks)
        {
            if (await task.ExecuteTaskAsync()) continue;

            if (task.HasErrorMessage)
                await _UserDialog.ErrorMessageAsync(task.ErrorMessage!);
            _SuccessResult = false;

            if (BreakOnFail)
                break;
        }

        _Tasks.Clear();
        return _SuccessResult;
    }

    /// <summary>
    /// Очистить очередь задач
    /// </summary>
    public void ClearTasks() => _Tasks.Clear();


    internal interface IActionHelperTask
    {
        /// <summary> Выполнить задачу и вернуть успешно или нет </summary>
        Task<bool> ExecuteTaskAsync();

        /// <summary> Сообщение при неудаче </summary>
        string? ErrorMessage { get; }

        /// <summary> Выводить сообщение о неудаче </summary>
        bool HasErrorMessage => ErrorMessage != null;

        /// <summary> Добавить другую задачу </summary>
        public ActionHelper Then();

        /// <summary>
        /// Начать выполнение задач в очереди
        /// </summary>
        /// <param name="BreakOnFail">Прервать выполнение при неудачной проверке выполнения</param>
        /// <returns>True, если все задачи выполнены успешно</returns>
        public Task<bool> ExecuteAsync(bool BreakOnFail = true);
    }


    /// <summary>
    /// Задача для выполнения
    /// </summary>
    public class ActionHelperTask : IActionHelperTask
    {
        private readonly ActionHelper _ActionHelper;

        private readonly Func<Task<bool>> _ExecutingTask;
        private readonly string? _ErrorMessage;

        private Func<Task>? OnSuccessAction { get; set; }
        private Func<Task>? OnFailAction { get; set; }


        public ActionHelperTask(ActionHelper ActionHelper, Func<Task<bool>> ExecutingTask, string? OnFailMessage)
        {
            _ErrorMessage = OnFailMessage;
            _ActionHelper = ActionHelper;
            _ExecutingTask = ExecutingTask;
        }


        public  ActionHelper Then() => _ActionHelper;


        /// <summary>
        /// Начать выполнение задач в очереди
        /// </summary>
        /// <param name="BreakOnFail">Прервать выполнение при неудачной проверке выполнения</param>
        /// <returns>True, если все задачи выполнены успешно</returns>
        public Task<bool> ExecuteAsync(bool BreakOnFail = true) => _ActionHelper.ExecuteAsync(BreakOnFail);


        /// <summary>
        /// Выполнить действие при успехе этой задачи
        /// </summary>
        /// <param name="Action">Действие</param>
        public ActionHelperTask OnSuccess(Action Action)
        {
            OnSuccessAction = () =>
            {
                Action.Invoke();
                return Task.CompletedTask;
            };
            return this;
        }


        /// <summary>
        /// Выполнить другую задачу при успехе этой задачи
        /// </summary>
        /// <param name="Action">Задача</param>
        public ActionHelperTask OnSuccess(Func<Task> Action)
        {
            OnSuccessAction = Action;
            return this;
        }


        /// <summary>
        /// Выполнить действие при провале этой задачи
        /// </summary>
        /// <param name="Action">Действие</param>
        public ActionHelperTask OnFail(Action Action)
        {
            OnFailAction = () =>
            {
                Action.Invoke();
                return Task.CompletedTask;
            };
            return this;
        }


        /// <summary>
        /// Выполнить другую задачу при провале этой задачи
        /// </summary>
        /// <param name="Action">Задача</param>
        public ActionHelperTask OnFail(Func<Task> Action)
        {
            OnFailAction = Action;
            return this;
        }


        async Task<bool> IActionHelperTask.ExecuteTaskAsync()
        {
            var result = await _ExecutingTask.Invoke();
            if (result)
                OnSuccessAction?.Invoke();
            else
                OnFailAction?.Invoke();
            return result;
        }

        string? IActionHelperTask.ErrorMessage => _ErrorMessage;
    }

    /// <summary> Типизированная задача с возможностью обработки результата выполнения </summary>
    public class ActionHelperTask<T> : IActionHelperTask
    {
        public readonly struct ActionResult
        {
            public bool IsSuccess { get; }
            public T Result { get; }

            public ActionResult(bool IsSuccess, T result)
            {
                this.IsSuccess = IsSuccess;
                Result = result;
            }
        }


        private readonly ActionHelper _ActionHelper;
        private readonly Func<Task<ActionResult>> _ExecutingTask;
        private readonly string? _OnFailMessage;

        private Func<T, string>? OnSuccessQuestionMessage { get; set; }

        private Func<T, string>? OnFailQuestionMessage { get; set; }

        private Func<T, Task>? OnSuccessAction { get; set; }
        private Func<T, Task>? OnFailAction { get; set; }


        public ActionHelperTask(ActionHelper ActionHelper, Func<Task<ActionResult>> ExecutingTask, string? OnFailMessage)
        {
            _ActionHelper = ActionHelper;
            _ExecutingTask = ExecutingTask;
            _OnFailMessage = OnFailMessage;
        }

        async Task<bool> IActionHelperTask.ExecuteTaskAsync()
        {
            var result = await _ExecutingTask.Invoke();
            var success = result.IsSuccess;
            if (success)
            {
                if (OnSuccessAction is { } action)
                    await action.Invoke(result.Result);

                if (OnSuccessQuestionMessage is { } msg)
                    success = await _ActionHelper._UserDialog.QuestionAsync(msg.Invoke(result.Result));
            }
            else
            { 
                if (OnFailAction is { } faliAction)
                    await faliAction.Invoke(result.Result);

                if (OnFailQuestionMessage is { } msg)
                    success = await _ActionHelper._UserDialog.QuestionAsync(msg.Invoke(result.Result));
            }

            return success;
        }

        /// <summary>
        /// Выполнить действие при успехе этой задачи
        /// </summary>
        /// <param name="Action">Действие</param>
        public ActionHelperTask<T> OnSuccess(Action<T> Action)
        {
            OnSuccessAction = t =>
            {
                Action.Invoke(t);
                return Task.CompletedTask;
            };
            return this;
        }


        /// <summary>
        /// Выполнить другую задачу при успехе этой задачи
        /// </summary>
        /// <param name="Action">Задача</param>
        public ActionHelperTask<T> OnSuccess(Func<T, Task> Action)
        {
            OnSuccessAction = Action;
            return this;
        }


        /// <summary>
        /// Вопрос при успешном исполнении задачи.
        /// Результат исполнения задачи будет изменён в соответствии с ответом пользователя
        /// </summary>
        public ActionHelperTask<T> OnSuccessQuestion(Func<T, string> Message)
        {
            OnSuccessQuestionMessage = Message;
            return this;
        }


        /// <summary>
        /// Выполнить действие при провале этой задачи
        /// </summary>
        /// <param name="Action">Действие</param>
        public ActionHelperTask<T> OnFail(Action<T> Action)
        {
            OnFailAction = t =>
            {
                Action.Invoke(t);
                return Task.CompletedTask;
            };
            return this;
        }


        /// <summary>
        /// Выполнить другую задачу при провале этой задачи
        /// </summary>
        /// <param name="Action">Задача</param>
        public ActionHelperTask<T> OnFail(Func<T, Task> Action)
        {
            OnFailAction = Action;
            return this;
        }

        /// <summary>
        /// Вопрос при провальном исполнении задачи.
        /// Результат исполнения задачи будет изменён в соответствии с ответом пользователя
        /// </summary>
        public ActionHelperTask<T> OnFailQuestion(Func<T, string> Message)
        {
            OnFailQuestionMessage = Message;
            return this;
        }

        string? IActionHelperTask.ErrorMessage => _OnFailMessage;

        public ActionHelper Then() => _ActionHelper;

        public Task<bool> ExecuteAsync(bool BreakOnFail = true) => _ActionHelper.ExecuteAsync(BreakOnFail);
    }
}