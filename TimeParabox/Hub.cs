namespace TimeParabox;

public class Hub {

    public string name { get; }
    public IList<ActionSequence> actionSequences { get; }

    public IEnumerable<Puzzle> puzzles => actionSequences.OfType<Puzzle>();

    public Hub(string name, params ActionSequence[] actionSequences) {
        this.name            = name;
        this.actionSequences = actionSequences;
    }

}