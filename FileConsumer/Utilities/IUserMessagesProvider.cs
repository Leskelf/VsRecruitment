using FileConsumer.Utilities.Enums;

namespace FileConsumer.Utilities;

public interface IUserMessagesProvider
{
    string GetString(MessageKey key);
}
