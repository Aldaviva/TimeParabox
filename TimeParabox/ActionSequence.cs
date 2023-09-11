namespace TimeParabox;

public interface ActionSequence {

    string actions { get; }
    int? leadingDelay { get; }
    int? trailingDelay { get; }

}

public record Puzzle(int id, string actions): ActionSequence {

    public int? leadingDelay { get; init; } = 50;
    public int? trailingDelay { get; init; } = 1700;

}

internal record InterPuzzleMovement(string actions): ActionSequence {

    public int? leadingDelay { get; init; } = null;
    public int? trailingDelay { get; init; } = 250;

}