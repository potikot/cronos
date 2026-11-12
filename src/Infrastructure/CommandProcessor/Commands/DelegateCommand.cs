using System.Reflection;
using Cronos.Infrastructure.Converter;
using Cronos.Utilities;

namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class DelegateCommand<T> : BaseCommand where T : Delegate
{
    private readonly IConverterService _converter;

    private readonly T _callback;
    private readonly ParameterInfo[] _parameterInfos;
    private readonly bool _acceptsArray;

    public override string Name { get; }
    public override string Description { get; }

    public DelegateCommand(string name, string description, T callback, IConverterService converter) 
    {
        Name = name;
        Description = description;
        
        _callback = callback;
        _converter = converter;
        
        _parameterInfos = callback.Method.GetParameters();
        _acceptsArray = _parameterInfos.Length == 1 && 
                             _parameterInfos[0].ParameterType.Match(Types.ObjectArray, Types.StringArray);
    }

    public override Task<CommandResult> ProcessAsync(string[] args)
    {
        if (!ValidateArguments(args))
            return Task.FromResult(CommandResult.Invalid);

        object[] parameters = PrepareParameters(args);
        if (parameters == null)
            return Task.FromResult(CommandResult.Invalid);

        try
        {
            _callback.Method.Invoke(_callback.Target, parameters);
            return Task.FromResult(CommandResult.Success);
        }
        catch (Exception)
        {
            return Task.FromResult(CommandResult.Invalid);
        }
    }

    private bool ValidateArguments(string[] args)
    {
        int expectedCount = _parameterInfos.Length;
        if (expectedCount == 0)
            return args is not { Length: not 0 };

        return _acceptsArray || args?.Length == expectedCount;
    }
    
    private object[] PrepareParameters(string[] args)
    {
        if (_parameterInfos.Length == 0) 
            return null;
        if (_acceptsArray)
            return [args ?? []];

        return ConvertParameters(args);
    }

    private object[] ConvertParameters(string[] args)
    {
        object[] parameters = new object[_parameterInfos.Length];
        for (int i = 0; i < _parameterInfos.Length; i++)
        {
            if (!_converter.TryConvert(_parameterInfos[i].ParameterType, args[i], out parameters[i]))
                return null;
        }
        
        return parameters;
    }
}