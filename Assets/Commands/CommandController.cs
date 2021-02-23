using System.Collections.Generic;
using UnityEngine;

public class CommandController : MonoBehaviour
{
    private List<Command> _commands = new List<Command>();

    public void ExecuteCommand(Command command)
    {
        _commands.Add(command);
        command.Execute();
    }

    public void UndoLatestCommand()
    {
        var nextCommand = _commands.Count - 1;
        if (nextCommand < 0) return;
        _commands[nextCommand].Undo();
        _commands.RemoveAt(nextCommand);
    }
}