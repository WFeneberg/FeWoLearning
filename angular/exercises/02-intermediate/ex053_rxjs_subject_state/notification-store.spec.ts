import { TestBed } from "@angular/core/testing";
import { Notice, NotificationStore } from "./notification-store";

const first: Notice = { id: 1, text: "one" };
const second: Notice = { id: 2, text: "two" };
const third: Notice = { id: 3, text: "three" };
const fourth: Notice = { id: 4, text: "four" };

describe("NotificationStore", () => {
  let store: NotificationStore;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    store = TestBed.inject(NotificationStore);
  });

  it("starts with no unread notices", () => {
    const seen: number[] = [];
    store.unreadCount$.subscribe((count) => seen.push(count));

    // A BehaviorSubject delivers its current value on subscribe, so this is immediate.
    expect(seen).toEqual([0]);
    expect(store.currentCount()).toBe(0);
  });

  it("counts an arrival", () => {
    const seen: number[] = [];
    store.unreadCount$.subscribe((count) => seen.push(count));

    store.notify(first);
    store.notify(second);

    expect(seen).toEqual([0, 1, 2]);
    expect(store.currentCount()).toBe(2);
  });

  it("gives a late subscriber the current count", () => {
    store.notify(first);
    store.notify(second);

    const seen: number[] = [];
    store.unreadCount$.subscribe((count) => seen.push(count));

    // The whole reason to use a BehaviorSubject for state: a component created after the fact
    // still renders correctly instead of showing nothing.
    expect(seen).toEqual([2]);
  });

  it("resets the count", () => {
    store.notify(first);
    store.markAllRead();

    expect(store.currentCount()).toBe(0);
  });

  it("emits arrivals to a listener that was already there", () => {
    const seen: Notice[] = [];
    store.arrivals$.subscribe((notice) => seen.push(notice));

    store.notify(first);
    store.notify(second);

    expect(seen).toEqual([first, second]);
  });

  it("gives a late subscriber none of the past arrivals", () => {
    store.notify(first);

    const seen: Notice[] = [];
    store.arrivals$.subscribe((notice) => seen.push(notice));

    // A plain Subject has no memory. Correct for events, and the source of the "empty on
    // refresh" bug when used for state.
    expect(seen).toEqual([]);

    store.notify(second);
    expect(seen).toEqual([second]);
  });

  it("replays the recent notices to a late subscriber", () => {
    store.notify(first);
    store.notify(second);

    const seen: Notice[] = [];
    store.recent$.subscribe((notice) => seen.push(notice));

    expect(seen).toEqual([first, second]);
  });

  it("keeps only the last three", () => {
    for (const notice of [first, second, third, fourth]) {
      store.notify(notice);
    }

    const seen: Notice[] = [];
    store.recent$.subscribe((notice) => seen.push(notice));

    expect(seen).toEqual([second, third, fourth]);
  });

  it("keeps feeding a replay subscriber after it joins", () => {
    store.notify(first);
    const seen: Notice[] = [];
    store.recent$.subscribe((notice) => seen.push(notice));

    store.notify(second);

    expect(seen).toEqual([first, second]);
  });

  it("seals the write end of every public stream", () => {
    expect(store.isSealed()).toBe(true);

    // Concretely: nothing outside the store can push a value in.
    expect((store.unreadCount$ as unknown as { next?: unknown }).next).toBeUndefined();
    expect((store.arrivals$ as unknown as { next?: unknown }).next).toBeUndefined();
    expect((store.recent$ as unknown as { next?: unknown }).next).toBeUndefined();
  });

  it("still lets the store itself write", () => {
    const seen: number[] = [];
    store.unreadCount$.subscribe((count) => seen.push(count));

    store.notify(first);

    // Sealed to the outside, not to the owner.
    expect(seen).toEqual([0, 1]);
  });

  it("delivers one notify to all three streams", () => {
    const counts: number[] = [];
    const arrivals: Notice[] = [];
    const recent: Notice[] = [];
    store.unreadCount$.subscribe((c) => counts.push(c));
    store.arrivals$.subscribe((n) => arrivals.push(n));
    store.recent$.subscribe((n) => recent.push(n));

    store.notify(first);

    expect(counts).toEqual([0, 1]);
    expect(arrivals).toEqual([first]);
    expect(recent).toEqual([first]);
  });
});
