namespace FileConsumer.Utilities;

using System.Resources;
using System.Globalization;
using FileConsumer.Utilities.Enums;

public class UserMessagesProvider : IUserMessagesProvider
{
    private readonly ResourceManager _resourceManager;

    public UserMessagesProvider(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public string GetString(MessageKey key)
        => _resourceManager.GetString(key.ToString(), CultureInfo.CurrentCulture) ?? $"[{key}]";
}
