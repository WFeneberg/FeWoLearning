using System;
using System.Threading;
using System.Threading.Tasks;

namespace FeWoLearning.Exercises.Expert;

// Exercise 093 — Blazor component with state (reference solution).
// A SemaphoreSlim(1, 1) stands in for Blazor's single-threaded renderer
// synchronization context: only one click handler body runs its
// "read count, do async work, mutate count, render" sequence at a time, so
// concurrent HandleClickAsync calls cannot race and lose an increment, and
// each completed click produces exactly one render.
public sealed class BlazorCounterComponent
{
    private readonly SemaphoreSlim _renderGate = new(1, 1);
    private int _count;

    public BlazorCounterComponent()
    {
        Render();
    }

    public int Count => _count;

    public int RenderCount { get; private set; }

    public string Markup { get; private set; } = string.Empty;

    public async Task HandleClickAsync()
    {
        await _renderGate.WaitAsync().ConfigureAwait(false);
        try
        {
            // Simulate awaiting async work (e.g. a service call) before the
            // state mutation is applied — the classic spot where a naive
            // implementation would race under concurrent clicks.
            await Task.Yield();
            _count++;
            Render();
        }
        finally
        {
            _renderGate.Release();
        }
    }

    private void Render()
    {
        RenderCount++;
        Markup = $"<div class=\"counter\"><p role=\"status\">Current count: {_count}</p><button>Click me</button></div>";
    }
}
