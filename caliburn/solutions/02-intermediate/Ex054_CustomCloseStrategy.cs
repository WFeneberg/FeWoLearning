// Exercise 054 - Custom Close Strategy (intermediate).
// Goal:   ICloseStrategy&lt;T&gt; has exactly one member - ExecuteAsync(IEnumerable&lt;T&gt;,
//         CancellationToken) -> Task&lt;ICloseResult&lt;T&gt;&gt; - and ConductorBase&lt;T&gt;.CloseStrategy
//         is a plain SETTABLE property, so a hand-written policy plugs in wherever Caliburn's own
//         DefaultCloseStrategy (ex053) would otherwise run.
// Drills: implementing ICloseStrategy&lt;T&gt; from scratch with a genuinely different decision rule -
//         a MAJORITY vote (more than half of the items individually willing) instead of Caliburn's
//         own all-or-nothing DefaultCloseStrategy - and assigning it to a real conductor's
//         CloseStrategy property.
// Passes: dotnet test --filter FullyQualifiedName~Ex054_

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
