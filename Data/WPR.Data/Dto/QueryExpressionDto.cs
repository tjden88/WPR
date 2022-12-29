using System.Linq.Expressions;
using System.Text.Json.Serialization;
using Serialize.Linq.Serializers;
namespace WPR.Data.Dto;

/// <summary> Класс - DTO для передачи выражений в контроллер в текстовом виде </summary>
public class QueryExpressionDto
{
    [JsonConstructor]
    public QueryExpressionDto()
    {
    }

    public string JsonString { get; set; }


    public QueryExpressionDto(Expression Expression) => SetExpression(Expression);

    public void SetExpression(Expression Expression)
    {
        var serializer = new ExpressionSerializer(new JsonSerializer());
        JsonString = serializer.SerializeText(Expression);
    }

    public Expression GetExpression()
    {
        var serializer = new ExpressionSerializer(new JsonSerializer());
        return serializer.DeserializeText(JsonString);
    }
}