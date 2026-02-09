using System.Text.Json;
using Workvivo.Shared.Models;
using Workvivo.Shared.Serialization;

namespace WorkvivoCli.Output;

public class JsonOutputFormatter : IOutputFormatter
{
    public void FormatUsers(IEnumerable<User> users)
    {
        var json = JsonSerializer.Serialize(users, AppJsonSerializerContext.Default.IEnumerableUser);
        Console.WriteLine(json);
    }

    public void FormatUser(User user)
    {
        var json = JsonSerializer.Serialize(user, AppJsonSerializerContext.Default.User);
        Console.WriteLine(json);
    }

    public void FormatSpaces(IEnumerable<Space> spaces)
    {
        var json = JsonSerializer.Serialize(spaces, AppJsonSerializerContext.Default.IEnumerableSpace);
        Console.WriteLine(json);
    }

    public void FormatSpace(Space space)
    {
        var json = JsonSerializer.Serialize(space, AppJsonSerializerContext.Default.Space);
        Console.WriteLine(json);
    }

    public void FormatSpaceUsers(IEnumerable<User> users)
    {
        var json = JsonSerializer.Serialize(users, AppJsonSerializerContext.Default.IEnumerableUser);
        Console.WriteLine(json);
    }
}
