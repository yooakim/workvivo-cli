namespace WorkvivoCli.Output;

public interface IOutputFormatter
{
    void FormatUsers(IEnumerable<Models.User> users);
    void FormatUser(Models.User user);
    void FormatSpaces(IEnumerable<Models.Space> spaces);
    void FormatSpace(Models.Space space);
    void FormatSpaceUsers(IEnumerable<Models.User> users);
}
