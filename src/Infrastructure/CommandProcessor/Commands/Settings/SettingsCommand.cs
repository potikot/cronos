using System.Reflection;
using Cronos.Features.Settings;
using Cronos.Infrastructure.Converter;
using Cronos.Infrastructure.Localization;
using Cronos.Infrastructure.Logger;

namespace Cronos.Infrastructure.CommandProcessor.Commands;

public class SettingsCommand : CompositeCommandBase
{
    private const BindingFlags PropertyBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty;
    private const BindingFlags MethodBindingFlags = BindingFlags.Instance | BindingFlags.Public;

    private readonly ILoggerService _logger;
    private readonly ISettingsService _settings;
    private readonly IConverterService _converter;

    public override string Name { get; } = "settings";
    public override string Description { get; } = "Sets the settings for the app.";

    public SettingsCommand(
        ILoggerService logger,
        ILocalizationService localization,
        ISettingsService settings,
        IConverterService converter)
    {
        _logger = logger;
        _settings = settings;
        _converter = converter;

        Type settingsType = settings.GetType();
        AddSettingsProperties(settingsType);
        AddSettingsMethods(settingsType);

        subCommands.Add(new ChangeLanguageCommand(logger, localization));
    }

    private void AddSettingsProperties(Type type)
    {
        PropertyInfo[] properties = type.GetProperties(PropertyBindingFlags);
        AddSettingsMembers(properties, HandleProperty);
    }

    private void AddSettingsMethods(Type type)
    {
        MethodInfo[] methods = type.GetMethods(MethodBindingFlags);
        AddSettingsMembers(methods, HandleMethod, mi => !mi.IsSpecialName);
    }

    private void AddSettingsMembers<T>(
        T[] memberInfos,
        Action<T, string[]> handler,
        Func<T, bool> validator = null) where T : MemberInfo
    {
        foreach (T memberInfo in memberInfos)
        {
            if (validator != null && !validator(memberInfo)) continue;
            SettingAttribute attribute = memberInfo.GetCustomAttribute<SettingAttribute>();
            if (attribute == null) continue;
            
            string name = string.IsNullOrWhiteSpace(attribute.Name) ? memberInfo.Name : attribute.Name;
            subCommands.Add(new SettingCommand(name, attribute.Description, args => handler(memberInfo, args), _converter));
        }
    }
    
    private void HandleProperty(PropertyInfo propertyInfo, string[] args)
    {
        if (args is not { Length: not 0 })
        {
            _logger.LogInfo($"{propertyInfo.Name} is {propertyInfo.GetValue(_settings)}");
            return;
        }
        
        if (_converter.TryConvert(propertyInfo.PropertyType, args[0], out object value))
            propertyInfo.SetValue(_settings, value);
        else
            _logger.LogError(this, $"Unable to convert '{args[0]}' to '{propertyInfo.PropertyType}'.");
    }
    
    private void HandleMethod(MethodInfo methodInfo, string[] args)
    {
        ParameterInfo[] parameterInfos = methodInfo.GetParameters();
        if (args?.Length != parameterInfos.Length)
            return;
        
        object[] parameters = new object[parameterInfos.Length];
        for (int i = 0; i < args.Length; i++)
        {
            if (_converter.TryConvert(parameterInfos[i].ParameterType, args[i], out object value))
                parameters[i] = value;
            else
            {
                _logger.LogError(this, $"Unable to convert '{args[0]}' to '{parameterInfos[i].ParameterType}'.");
                return;
            }
        }
        
        methodInfo.Invoke(_settings, parameters);
    }

    private class SettingCommand(string name, string description, Action<string[]> callback, IConverterService converter)
        : DelegateCommand<Action<string[]>>(name, description, callback, converter);
}