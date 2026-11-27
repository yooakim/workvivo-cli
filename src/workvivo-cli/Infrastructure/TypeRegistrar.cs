using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace WorkvivoCli.Infrastructure;

/// <summary>
/// Bridges Spectre.Console.Cli with Microsoft.Extensions.DependencyInjection.
/// Implements <see cref="ITypeRegistrar"/> so that Spectre.Console.Cli can
/// register command types in the DI container and resolve them with full
/// constructor injection support.
/// </summary>
public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    public TypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_builder.BuildServiceProvider());
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067",
        Justification = "Spectre.Console.Cli's ITypeRegistrar does not annotate the 'implementation' parameter. The types registered are command types whose public constructors are preserved by the DI container.")]
    public void Register(Type service, Type implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> func)
    {
        _builder.AddSingleton(service, _ => func());
    }
}
