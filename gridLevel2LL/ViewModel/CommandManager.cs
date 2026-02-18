using gridLevel2LL.Commands;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gridLevel2LL.ViewModel
{
    internal class CommandManager
    {
        private Stack<IGridCommand> undoStack = new Stack<IGridCommand>();
        private Stack<IGridCommand> redoStack = new Stack<IGridCommand>();

        public bool canUndo => undoStack.Count > 0;
        public bool canRedo => redoStack.Count > 0;

        public void Execute(IGridCommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
        }

        public void Undo()
        {
            if (!canUndo)
            {
                return;
            }

            IGridCommand command = undoStack.Pop();
            command.Undo();
            redoStack.Push(command);
        }


        public void Redo()
        {
            if (!canRedo)
            {
                return;
            }

            IGridCommand command = redoStack.Pop();
            command.Execute();
            undoStack.Push(command);
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();

        }
    }
}
