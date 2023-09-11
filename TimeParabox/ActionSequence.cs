namespace TimeParabox;

public interface ActionSequence {

    string actions { get; }
    int? leadingDelay { get; }
    int? trailingDelay { get; }

}

public record Puzzle(int id, string actions): ActionSequence {

    // 35 1610 fails (not between puzzles)
    public int? leadingDelay { get; init; } = 300;   //17 * 6;
    public int? trailingDelay { get; init; } = 1750; // 1610 fails (with 25 leading)

}

internal record InterPuzzleMovement(string actions): ActionSequence {

    public int? leadingDelay { get; init; } = null;
    public int? trailingDelay { get; init; } = 300;

}