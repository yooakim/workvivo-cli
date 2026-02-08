using System.Text.Json;
using WorkvivoCli.Serialization;

namespace WorkvivoCli.Output;

public class JsonOutputFormatter : IOutputFormatter
{
    public void FormatUsers(IEnumerable<Models.User> users)
    {
        var json = JsonSerializer.Serialize(users, AppJsonSerializerContext.Default.IEnumerableUser);
        Console.WriteLine(json);
    }

    public void FormatUser(Models.User user)
    {
        var json = JsonSerializer.Serialize(user, AppJsonSerializerContext.Default.User);
        Console.WriteLine(json);
    }

    public void FormatSpaces(IEnumerable<Models.Space> spaces)
    {
        var json = JsonSerializer.Serialize(spaces, AppJsonSerializerContext.Default.IEnumerableSpace);
        Console.WriteLine(json);
    }

    public void FormatSpace(Models.Space space)
    {
        var json = JsonSerializer.Serialize(space, AppJsonSerializerContext.Default.Space);
        Console.WriteLine(json);
    }

    public void FormatSpaceUsers(IEnumerable<Models.User> users)
    {
        var json = JsonSerializer.Serialize(users, AppJsonSerializerContext.Default.IEnumerableUser);
        Console.WriteLine(json);
    }
}
