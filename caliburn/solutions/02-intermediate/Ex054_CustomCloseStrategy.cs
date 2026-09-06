// Exercise 054 - Custom Close Strategy (intermediate).
// Goal:   ICloseStrategy<T> has exactly one member - ExecuteAsync(IEnumerable<T>,
//         CancellationToken) -> Task<ICloseResult<T>> - and ConductorBase<T>.CloseStrategy is a
//         plain SETTABLE property, so a hand-written policy plugs in wherever Caliburn's own
//         DefaultCloseStrategy (ex053) would otherwise run.
// Drills: implementing ICloseStrategy<T> from scratch with a genuinely different decision rule -
//         a MAJORITY vote (more than half of the items individually willing) instead of Caliburn's
//         own all-or-nothing DefaultCloseStrategy - and assigning it to a real conductor's
//         CloseStrategy property.
// Passes: dotnet test --filter FullyQualifiedName~Ex054_
//
// Sharp edge, measured: when the vote LOSES, this strategy's Children still holds whoever was
// individually willing - and a real conductor's CanCloseAsync deactivates (close: true) and
// removes exactly those children from Items as a side effect of merely being asked, even though
// CloseCanOccur came back false for the group as a whole. CanCloseAsync is not a pure query in
// general - see ex052's own corrected claim, scoped to the DEFAULT strategy, where this same side
// effect cannot happen because the default strategy's Children is empty on any refusal.

using System.Linq;
using System.Threading;
using Caliburn.Micro;

namespace FeWoLearning.Caliburn.Exercises.Intermediate;

/// <summary>A screen whose CanCloseAsync answer is set directly by the test - the vote this
/// exercise's strategy counts.</summary>
public class Ex054_Item : Screen
{
    public bool RefuseClose { get; set; }

    public override Task<bool> CanCloseAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(!RefuseClose);
}

/// <summary>A hand-written ICloseStrategy&lt;T&gt;: CloseCanOccur is true only when MORE THAN
/// HALF of toClose individually agree to close - a genuinely different policy from Caliburn's own
/// DefaultCloseStrategy, which requires every single one to agree. Children is always the willing
/// subset, regardless of whether the majority was actually reached.</summary>
public class Ex054_MajorityRulesCloseStrategy : ICloseStrategy<Ex054_Item>
{
    public async Task<ICloseResult<Ex054_Item>> ExecuteAsync(IEnumerable<Ex054_Item> toClose, CancellationToken cancellationToken = default)
    {
        var items = toClose.ToList();
        var willing = new List<Ex054_Item>();

        foreach (var item in items)
        {
            if (await item.CanCloseAsync(cancellationToken))
                willing.Add(item);
        }

        var closeCanOccur = willing.Count * 2 > items.Count;
        return new CloseResult<Ex054_Item>(closeCanOccur, willing);
    }
}
