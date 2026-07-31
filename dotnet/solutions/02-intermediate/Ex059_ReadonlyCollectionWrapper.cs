using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FeWoLearning.Exercises.Intermediate;

// Exercise 059 — Readonly Collection Wrapper (reference solution).
public class ReadonlyCollectionWrapper
{
    private readonly List<string> _items = new();
    private readonly ReadOnlyCollection<string> _view;

    public ReadonlyCollectionWrapper()
    {
        // ReadOnlyCollection<T> keeps a reference to the underlying IList<T>
        // rather than copying it, so this view stays live: later additions
        // made through Add() are visible through Items without re-wrapping.
        _view = new ReadOnlyCollection<string>(_items);
    }

    public IReadOnlyList<string> Items => _view;

    public int Count => _items.Count;

    public void Add(string item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }
}
