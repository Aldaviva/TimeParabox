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

        string? startingHubName     = args.ElementAtOrDefault(0);
        int?    startingPuzzleId    = args.ElementAtOrDefault(1) is { } rawPuzzleId ? int.Parse(rawPuzzleId) : null;
        bool    continueAfterPuzzle = true;
        bool    continueAfterHub    = true;

        Hub     startingHub    = startingHubName != null ? Puzzles.HUBS.First(hub => hub.name == startingHubName) : Puzzles.HUBS[0];
        Puzzle? startingPuzzle = startingPuzzleId != null ? startingHub.puzzles.First(puzzle => puzzle.id == startingPuzzleId) : null;

        foreach (Hub hub in Puzzles.HUBS.SkipWhile(hub => !ReferenceEquals(hub, startingHub))) {
            IEnumerable<ActionSequence> actionSequences = hub == startingHub && startingPuzzle != null
                ? hub.actionSequences.SkipWhile(puzzle => !ReferenceEquals(puzzle, startingPuzzle))
                : hub.actionSequences;

            foreach (ActionSequence actionSequence in actionSequences) {
                int leadingDelay = actionSequence == hub.actionSequences.Last() && actionSequence.leadingDelay == null
                    ? Puzzles.hubCompletionDelay(hub.actionSequences
                        .Reverse()
                        .SkipWhile(sequence => sequence is InterPuzzleMovement)
                        .TakeWhile(sequence => sequence is Puzzle).Count())
                    : actionSequence.leadingDelay ?? 0;
                if (leadingDelay > 0) {
                    Console.WriteLine($"Sleeping for {leadingDelay:N0} ms (leading delay)");
                    Thread.Sleep(leadingDelay);
                }

                Console.WriteLine($"{hub.name} {(actionSequence as Puzzle)?.id.ToString() ?? "between puzzles"}: {actionSequence.actions}");
                sendCommands(actionSequence.actions);

                int trailingDelay = actionSequence.trailingDelay ?? 0;
                if (trailingDelay > 0) {
                    Console.WriteLine($"Sleeping for {trailingDelay:N0} ms (trailing delay)");
                    Thread.Sleep(trailingDelay);
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
        Thread.Sleep(SLOW_MOTION ? 250 : INTER_KEY_DELAY_MS);
    }

}