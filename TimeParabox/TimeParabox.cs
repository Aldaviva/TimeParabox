using System.Diagnostics;
using ManagedWinapi.Windows;
using SimWinInput;

namespace TimeParabox;

internal static class TimeParabox {

    private const int  INTER_KEY_DELAY_MS = 35;
    private const int  INTRA_KEY_DELAY_MS = 35;
    private const bool SLOW_MOTION        = false;

    public static void Main(string[] args) {
        SimGamePad.Instance.Initialize();
        SimGamePad.Instance.PlugIn();

        Console.WriteLine("Waiting for Patrick's Parabox window to be in the foreground...");
        while (!isGameWindow(SystemWindow.ForegroundWindow)) {
            Thread.Sleep(250);
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        string? startingHubName     = "Open";
        int?    startingPuzzleId    = null;
        bool    continueAfterPuzzle = true;
        bool    continueAfterHub    = false;

        Hub     startingHub    = startingHubName != null ? Puzzles.HUBS.First(hub => hub.name == startingHubName) : Puzzles.HUBS[0];
        Puzzle? startingPuzzle = startingPuzzleId != null ? startingHub.puzzles.First(puzzle => puzzle.id == startingPuzzleId) : null;

        foreach (Hub hub in Puzzles.HUBS.SkipWhile(hub => !ReferenceEquals(hub, startingHub))) {
            IEnumerable<ActionSequence> actionSequences = hub == startingHub && startingPuzzle != null
                ? startingHub.actionSequences.SkipWhile(puzzle => !ReferenceEquals(puzzle, startingPuzzle))
                : hub.actionSequences;

            foreach (ActionSequence actionSequence in actionSequences) {
                if (actionSequence.leadingDelay > 0) {
                    Console.WriteLine($"Sleeping for {actionSequence.leadingDelay:N0} ms (leading delay)");
                    Thread.Sleep(actionSequence.leadingDelay);
                }

                Console.WriteLine($"{hub.name} {(actionSequence as Puzzle)?.id.ToString() ?? "between puzzles"}: {actionSequence.actions}");
                sendCommands(actionSequence.actions);

                if (actionSequence.trailingDelay > 0) {
                    Console.WriteLine($"Sleeping for {actionSequence.trailingDelay:N0} ms (trailing delay)");
                    Thread.Sleep(actionSequence.trailingDelay);
                }

                if (!continueAfterPuzzle) {
                    break;
                }
            }

            if (!continueAfterPuzzle || !continueAfterHub) {
                break;
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Done in {stopwatch.Elapsed:g}.");
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
        Thread.Sleep(SLOW_MOTION ? 500 : INTER_KEY_DELAY_MS);
    }

}