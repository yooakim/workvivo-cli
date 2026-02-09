using Workvivo.Shared.Models;

namespace WorkvivoCli.Output;

public interface IOutputFormatter
{
    void FormatUsers(IEnumerable<User> users);
    void FormatUser(User user);
    void FormatSpaces(IEnumerable<Space> spaces);
    void FormatSpace(Space space);
    void FormatSpaceUsers(IEnumerable<User> users);
}
