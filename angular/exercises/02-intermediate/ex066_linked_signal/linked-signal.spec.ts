import { signal } from "@angular/core";
import { linked, naiveLinked } from "./linked-signal";

describe("linked", () => {
  it("starts at the computed default", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);

    expect(selection.value()).toBe("tools-first");
    expect(selection.overridden()).toBe(false);
  });

  it("follows the source while untouched", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);

    category.set("toys");

    expect(selection.value()).toBe("toys-first");
  });

  it("accepts an override", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);

    selection.set("hammer");

    expect(selection.value()).toBe("hammer");
    expect(selection.overridden()).toBe(true);
  });

  it("keeps the override while the source is unchanged", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);
    selection.set("hammer");

    expect(selection.value()).toBe("hammer");
    expect(selection.value()).toBe("hammer");
  });

  it("discards the override when the source changes", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);
    selection.set("hammer");

    category.set("toys");

    // The whole point: a hammer is not a toy, so the stale choice is dropped.
    expect(selection.value()).toBe("toys-first");
    expect(selection.overridden()).toBe(false);
  });

  it("does not resurrect an override when the source comes back", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);
    selection.set("hammer");
    category.set("toys");

    category.set("tools");

    // Returning to the old category is a fresh start, not an undo.
    expect(selection.value()).toBe("tools-first");
  });

  it("accepts a new override after a reset by the source", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);
    selection.set("hammer");
    category.set("toys");

    selection.set("kite");

    expect(selection.value()).toBe("kite");
    expect(selection.overridden()).toBe(true);
  });

  it("resets on request", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);
    selection.set("hammer");

    selection.reset();

    expect(selection.value()).toBe("tools-first");
    expect(selection.overridden()).toBe(false);
  });

  it("survives an override equal to the default", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);

    selection.set("tools-first");

    // Same value, but explicitly chosen — which is a different state from never having chosen.
    expect(selection.overridden()).toBe(true);
    expect(selection.value()).toBe("tools-first");
  });

  it("works with a non-string source", () => {
    const page = signal(1);
    const rows = linked(page, (p) => p * 10);

    expect(rows.value()).toBe(10);
    rows.set(99);
    expect(rows.value()).toBe(99);

    page.set(2);
    expect(rows.value()).toBe(20);
  });

  it("is readable repeatedly without changing anything", () => {
    const category = signal("tools");
    const selection = linked(category, (c) => `${c}-first`);

    for (const _ of [1, 2, 3]) {
      expect(selection.value()).toBe("tools-first");
      expect(selection.overridden()).toBe(false);
    }
  });
});

describe("naiveLinked", () => {
  it("is writable", () => {
    const category = signal("tools");
    const selection = naiveLinked(category, (c) => `${c}-first`);

    selection.set("hammer");

    expect(selection()).toBe("hammer");
  });

  it("starts from the source", () => {
    const category = signal("tools");
    const selection = naiveLinked(category, (c) => `${c}-first`);

    expect(selection()).toBe("tools-first");
  });

  it("never resets, which is the bug", () => {
    const category = signal("tools");
    const selection = naiveLinked(category, (c) => `${c}-first`);
    selection.set("hammer");

    category.set("toys");

    // A hammer in the toys category, and nothing anywhere reports a problem.
    expect(selection()).toBe("hammer");
  });

  it("does not even follow the source when untouched", () => {
    const category = signal("tools");
    const selection = naiveLinked(category, (c) => `${c}-first`);

    category.set("toys");

    // Seeded once at construction — a plain signal has no link to anything.
    expect(selection()).toBe("tools-first");
  });
});
