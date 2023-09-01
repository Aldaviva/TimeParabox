using ManagedWinapi.Windows;
using SimWinInput;

namespace TimeParabox;

internal static class TimeParabox {

    private const int  INTER_KEY_DELAY_MS = 30;
    private const int  INTRA_KEY_DELAY_MS = 18;
    private const bool SLOW_MOTION        = false;

    public static void Main(string[] args) {
        SimGamePad.Instance.Initialize();
        SimGamePad.Instance.PlugIn();

        SystemWindow[] gameWindows = SystemWindow.FilterToplevelWindows(isGameWindow);
        if (gameWindows.FirstOrDefault() is { } gameWindow) {
            Foregrounder.Foregrounder.BringToForeground(gameWindow.HWnd);
        }

        Console.WriteLine("Waiting for Patrick's Parabox window to be in the foreground...");
        while (!isGameWindow(SystemWindow.ForegroundWindow)) {
            Thread.Sleep(500);
        }

        string? startingHubName     = "Swap";
        int?    startingPuzzleId    = null;
        bool    continueAfterPuzzle = true;
        bool    continueAfterHub    = true;

        Hub     startingHub    = startingHubName != null ? Puzzles.hubs.First(hub => hub.name == startingHubName) : Puzzles.hubs[0];
        Puzzle? startingPuzzle = startingPuzzleId != null ? startingHub.puzzles.First(puzzle => puzzle.id == startingPuzzleId) : null;

        foreach (Hub hub in Puzzles.hubs.SkipWhile(hub => !ReferenceEquals(hub, startingHub))) {
            IEnumerable<ActionSequence> actionSequences = hub == startingHub && startingPuzzle != null
                ? startingHub.actionSequences.SkipWhile(puzzle => !ReferenceEquals(puzzle, startingPuzzle))
                : hub.actionSequences;

            foreach (ActionSequence actionSequence in actionSequences) {
                Thread.Sleep(actionSequence.leadingDelay);
                Console.WriteLine($"{hub.name} {(actionSequence as Puzzle)?.id.ToString() ?? "between puzzles"}: {actionSequence.actions}");
                sendCommands(actionSequence.actions);
                Thread.Sleep(actionSequence.trailingDelay);

                if (!continueAfterPuzzle) {
                    break;
                }
            }

            if (!continueAfterPuzzle || !continueAfterHub) {
                break;
            }
        }

        SimGamePad.Instance.ShutDown();
    }

    private static bool isGameWindow(SystemWindow window) {
        return window is { Title: "Patrick's Parabox", ClassName: "UnityWndClass" };
    }

    private static void sendCommands(string arrows) {
        foreach (char arrow in arrows) {
            sendCommand(arrow switch {
                'v' => GamePadControl.DPadDown,
                '^' => GamePadControl.DPadUp,
                '<' => GamePadControl.DPadLeft,
                '>' => GamePadControl.DPadRight,
                'a' => GamePadControl.A,
                's' => GamePadControl.Start
            });
        }
    }

    private static void sendCommand(GamePadControl control) {
        SimGamePad.Instance.Use(control, holdTimeMS: INTRA_KEY_DELAY_MS);
        Thread.Sleep(SLOW_MOTION ? 750 : INTER_KEY_DELAY_MS);
    }

}