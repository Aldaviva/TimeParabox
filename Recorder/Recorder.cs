using System.Text;
using Gma.System.MouseKeyHook;
using ManagedWinapi.Windows;

namespace Recorder;

internal static class Recorder {

    private static readonly List<InputAction> ACTIONS = new();
    private static readonly List<InputAction> UNDO    = new();

    [STAThread]
    public static void Main() {
        using IKeyboardMouseEvents mouseKeyHook = Hook.GlobalEvents();

        mouseKeyHook.KeyDown += (_, eventArgs) => {
            eventArgs.Handled = false;

            if (SystemWindow.ForegroundWindow is not { Title: "Patrick's Parabox", ClassName: "UnityWndClass" }) {
                return;
            }

            int lastIndex;
            switch (eventArgs.KeyCode) {
                case Keys.Up:
                    ACTIONS.Add(InputAction.UP);
                    UNDO.Clear();
                    break;
                case Keys.Right:
                    ACTIONS.Add(InputAction.RIGHT);
                    UNDO.Clear();
                    break;
                case Keys.Down:
                    ACTIONS.Add(InputAction.DOWN);
                    UNDO.Clear();
                    break;
                case Keys.Left:
                    ACTIONS.Add(InputAction.LEFT);
                    UNDO.Clear();
                    break;
                case Keys.Z:
                    lastIndex = ACTIONS.Count - 1;
                    if (lastIndex != -1) {
                        UNDO.Add(ACTIONS[lastIndex]);
                        ACTIONS.RemoveAt(lastIndex);
                    }

                    break;
                case Keys.Y:
                    lastIndex = UNDO.Count - 1;
                    if (lastIndex != -1) {
                        ACTIONS.Add(UNDO[lastIndex]);
                        UNDO.RemoveAt(lastIndex);
                    }

                    break;
                case Keys.R:
                    ACTIONS.Add(InputAction.RESET);
                    break;
                case Keys.N:
                    ACTIONS.Clear();
                    UNDO.Clear();
                    break;
                default:
                    return;
            }

            printActions();
        };

        Application.Run(new ApplicationContext());
    }

    private static void printActions() {
        StringBuilder stringBuilder = new();

        for (int index = ACTIONS.LastIndexOf(InputAction.RESET) + 1; index < ACTIONS.Count; index++) {
            InputAction inputAction = ACTIONS[index];
            stringBuilder.Append(inputAction switch {
                InputAction.DOWN  => "v",
                InputAction.UP    => "^",
                InputAction.LEFT  => "<",
                InputAction.RIGHT => ">",
                _                 => string.Empty
            });
        }

        Console.WriteLine(stringBuilder.ToString());
    }

}