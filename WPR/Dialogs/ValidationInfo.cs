namespace WPR.Dialogs;

/// <summary> Правила валидации </summary>
public record ValidationInfo(Predicate<string> Validated, string Message);
