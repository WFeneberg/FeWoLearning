namespace FeWoLearning.Exercises.Beginner;

// Exercise 035 — Queue/Stack Simulation (reference solution).
public static class QueueStackSimulation
{
    public static List<string> ProcessAll(Queue<string> printQueue, Stack<string> undoHistory)
    {
        var processed = new List<string>();

        while (printQueue.Count > 0)
        {
            string job = printQueue.Dequeue();
            processed.Add(job);
            undoHistory.Push(job);
        }

        return processed;
    }

    public static string? Undo(Stack<string> undoHistory)
        => undoHistory.Count > 0 ? undoHistory.Pop() : null;
}
