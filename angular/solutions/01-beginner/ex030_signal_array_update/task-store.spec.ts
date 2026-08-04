import { TestBed } from "@angular/core/testing";
import { Task, TaskStore } from "./task-store";

const task = (id: number, title: string, done = false): Task => ({ id, title, done });

describe("TaskStore", () => {
  let store: TaskStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(TaskStore);
  });

  it("starts empty and follows the first task", () => {
    expect(store.tasks()).toEqual([]);
    expect(store.openCount()).toBe(0);

    store.add(task(1, "write"));

    expect(store.openCount()).toBe(1);
  });

  it("appends a task", () => {
    store.add(task(1, "write"));

    expect(store.tasks()).toEqual([task(1, "write")]);
  });

  it("replaces the array rather than mutating it", () => {
    store.add(task(1, "write"));
    const before = store.tasks();

    store.add(task(2, "review"));

    expect(store.tasks()).not.toBe(before);
    // Whoever was holding the old array still sees exactly what they saw.
    expect(before).toHaveLength(1);
  });

  it("rejects a duplicate id", () => {
    store.add(task(1, "write"));

    expect(() => store.add(task(1, "again"))).toThrow(RangeError);
    expect(store.tasks()).toHaveLength(1);
  });

  it("removes by id", () => {
    store.add(task(1, "write"));
    store.add(task(2, "review"));

    store.remove(1);

    expect(store.tasks()).toEqual([task(2, "review")]);
  });

  it("ignores removing an unknown id", () => {
    store.add(task(1, "write"));

    store.remove(99);

    expect(store.tasks()).toEqual([task(1, "write")]);
  });

  it("toggles one task", () => {
    store.add(task(1, "write"));
    store.add(task(2, "review"));

    store.toggle(1);

    expect(store.tasks()[0].done).toBe(true);
    expect(store.tasks()[1].done).toBe(false);
  });

  it("leaves the untouched entries as the same objects", () => {
    store.add(task(1, "write"));
    store.add(task(2, "review"));
    const untouched = store.tasks()[1];

    store.toggle(1);

    // map() rebuilds the array but not the items, so this is still the same object.
    expect(store.tasks()[1]).toBe(untouched);
  });

  it("renames a task", () => {
    store.add(task(1, "write"));

    store.rename(1, "write tests");

    expect(store.tasks()[0].title).toBe("write tests");
  });

  it("refuses a blank title", () => {
    store.add(task(1, "write"));

    expect(() => store.rename(1, "   ")).toThrow(RangeError);
    expect(store.tasks()[0].title).toBe("write");
  });

  it("moves a task", () => {
    store.add(task(1, "a"));
    store.add(task(2, "b"));
    store.add(task(3, "c"));

    store.move(0, 2);

    expect(store.tasks().map((t) => t.id)).toEqual([2, 3, 1]);
  });

  it("moves a task backwards", () => {
    store.add(task(1, "a"));
    store.add(task(2, "b"));
    store.add(task(3, "c"));

    store.move(2, 0);

    expect(store.tasks().map((t) => t.id)).toEqual([3, 1, 2]);
  });

  it("rejects an out-of-range move", () => {
    store.add(task(1, "a"));

    expect(() => store.move(0, 5)).toThrow(RangeError);
    expect(() => store.move(-1, 0)).toThrow(RangeError);
  });

  it("sorts without disturbing the stored order", () => {
    store.add(task(1, "zebra"));
    store.add(task(2, "aardvark"));

    expect(store.sortedByTitle().map((t) => t.id)).toEqual([2, 1]);
    // sort() would have reordered the signal's own array in place.
    expect(store.tasks().map((t) => t.id)).toEqual([1, 2]);
  });

  it("counts the open tasks", () => {
    store.add(task(1, "a"));
    store.add(task(2, "b", true));

    expect(store.openCount()).toBe(1);
  });

  it("recomputes the count after a real update", () => {
    store.add(task(1, "a"));
    expect(store.openCount()).toBe(1);
    const before = store.recomputes;

    store.add(task(2, "b"));

    expect(store.openCount()).toBe(2);
    expect(store.recomputes).toBe(before + 1);
  });

  it("does not notice a mutated array", () => {
    store.add(task(1, "a"));
    expect(store.openCount()).toBe(1);
    const before = store.recomputes;

    store.addByMutating(task(2, "b"));

    // The item is genuinely in there...
    expect(store.tasks()).toHaveLength(2);

    // ...but the reference never changed, so nothing downstream was told. This is the bug
    // that looks like "signals are broken".
    expect(store.openCount()).toBe(1);
    expect(store.recomputes).toBe(before);
  });

  it("recovers once a proper update happens", () => {
    store.add(task(1, "a"));

    // The read has to happen *before* the mutation. A computed that has never been
    // evaluated has nothing cached, so it would simply see the mutated array and look
    // perfectly correct — the staleness only exists once there is a cached value.
    expect(store.openCount()).toBe(1);

    store.addByMutating(task(2, "b"));
    expect(store.openCount()).toBe(1);

    store.add(task(3, "c"));

    // The smuggled-in item shows up now, which is what makes the bug so confusing.
    expect(store.openCount()).toBe(3);
  });
});
