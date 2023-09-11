using System.Diagnostics;
using ManagedWinapi.Windows;
using SimWinInput;

namespace TimeParabox;

internal static class TimeParabox {

    // 17 17 works (G7)
    private const int  INTER_KEY_DELAY_MS    = 17;
    private const int  INTRA_KEY_DELAY_MS    = 17;
    private const bool SLOW_MOTION           = false;
    private const bool CONTINUE_AFTER_HUB    = true;
    private const bool CONTINUE_AFTER_PUZZLE = true;

    public static void Main(string[] args) {
        SimGamePad.Instance.Initialize();
        SimGamePad.Instance.PlugIn();

        Console.WriteLine("Waiting for Patrick's Parabox window to be in the foreground...");
        SystemWindow? gameWindow;
        while (null == (gameWindow = getGameWindow(SystemWindow.ForegroundWindow))) {
            Thread.Sleep(250);
        }

        using (Process selfProcess = Process.GetCurrentProcess()) {
            selfProcess.PriorityClass = ProcessPriorityClass.High;
        }

        using (Process gameProcess = gameWindow.Process) {
            gameProcess.PriorityClass = ProcessPriorityClass.High;
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        string? startingHubName  = args.ElementAtOrDefault(0);
        int?    startingPuzzleId = args.ElementAtOrDefault(1) is { } rawPuzzleId ? int.Parse(rawPuzzleId) : null;

        Hub     startingHub    = startingHubName != null ? Puzzles.HUBS.First(hub => hub.name.Equals(startingHubName, StringComparison.CurrentCultureIgnoreCase)) : Puzzles.HUBS[0];
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

                if (!CONTINUE_AFTER_PUZZLE) {
                    break;
                }
            }

            Console.WriteLine($"{hub.name} done in {stopwatch.Elapsed:g}.");

            if (!CONTINUE_AFTER_PUZZLE || !CONTINUE_AFTER_HUB) {
                break;
            }
        }

        stopwatch.Stop();
        Console.WriteLine($"Done in {stopwatch.Elapsed:g}.");
        SimGamePad.Instance.ShutDown();
    }

    private static SystemWindow? getGameWindow(SystemWindow window) => window is { Title: "Patrick's Parabox", ClassName: "UnityWndClass" } ? window : null;

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