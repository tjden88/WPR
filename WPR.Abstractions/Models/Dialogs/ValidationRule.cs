namespace WPR.Abstractions.Models.Dialogs;

/// <summary> Правила валидации </summary>
public record ValidationRule(Predicate<string?> Rule, string ErrorMessage);