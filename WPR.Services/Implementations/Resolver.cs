using Microsoft.Extensions.DependencyInjection;
using WPR.Abstractions.Interfaces;

namespace WPR.Services.Implementations;

internal class Resolver<T>(IServiceProvider serviceProvider) : IResolver<T> where T : notnull
{
    public T GetValue() => serviceProvider.GetRequiredService<T>();
}