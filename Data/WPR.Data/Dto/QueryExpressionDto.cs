using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Serialize.Linq.Serializers;
namespace WPR.Data.Dto;

/// <summary> Класс - DTO для передачи выражений в контроллер в текстовом виде </summary>
public class QueryExpressionDto
{
    private static readonly ExpressionSerializer _Serializer = new(new JsonSerializer());

    [JsonConstructor]
    public QueryExpressionDto()
    {
    }
    public QueryExpressionDto(Expression Expression) => SetExpression(Expression);


    /// <summary> Сериализованная строка запроса </summary>
    [JsonInclude]
    public string? JsonString { get; private set; }

    /// <summary> Сериализовать выражение LINQ </summary>
    public void SetExpression(Expression Expression) => 
        JsonString = _Serializer.SerializeText(Expression);

    /// <summary> Получить выражение LINQ этого экземпляра </summary>
    public Expression? GetExpression() =>
        string.IsNullOrEmpty(JsonString) 
            ? null
            : _Serializer.DeserializeText(JsonString);
}