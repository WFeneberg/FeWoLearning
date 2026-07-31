namespace FeWoLearning.Exercises.Beginner;

// Exercise 035 — Queue/Stack Simulation (beginner).
// Goal:   Simulate a print queue using Queue<T> for FIFO job processing
//         and a Stack<T> for undo history.
// Drills: Queue<T>, Stack<T>, FIFO vs LIFO ordering.
public static class QueueStackSimulation
{
    // Processes all jobs in the print queue in FIFO order, pushing each
    // processed job onto the undo history stack. Returns the jobs in the
    // order they were processed.
    public static List<string> ProcessAll(Queue<string> printQueue, Stack<string> undoHistory)
        => throw new NotImplementedException();

    // Undoes the most recently processed job by popping it off the undo
    // history stack and returning it. Returns null if there is nothing to undo.
    public static string? Undo(Stack<string> undoHistory)
        => throw new NotImplementedException();
}
