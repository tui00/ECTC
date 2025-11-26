using System.Collections.Generic;

namespace Ectc.Tools
{
    public interface ITool
    {
        string FullName { get; }
        string Command { get; }

        int Run(string[] args, List<string> rootCommand);
    }
}
