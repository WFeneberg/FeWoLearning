import { describe, expect, it, vi } from "vitest";
import { createStream, createSubject } from "@ex/03-advanced/ex080_observable_lite/index.js";

describe("ex080 createStream", () => {
  it("delivers values to the observer", () => {
    const stream = createStream(({ next, complete }) => {
      next(1);
      next(2);
      complete();
    });
    const seen = [];
    const done = vi.fn();
    stream.subscribe({ next: (value) => seen.push(value), complete: done });
    expect(seen).toEqual([1, 2]);
    expect(done).toHaveBeenCalledTimes(1);
  });

  it("is cold — the producer runs once per subscriber", () => {
    const producer = vi.fn(({ next }) => {
      next("value");
    });
    const stream = createStream(producer);
    stream.subscribe({ next: () => {} });
    stream.subscribe({ next: () => {} });
    expect(producer).toHaveBeenCalledTimes(2);
  });

  it("runs the teardown on unsubscribe, exactly once", () => {
    const teardown = vi.fn();
    const unsubscribe = createStream(() => teardown).subscribe({});
    expect(teardown).not.toHaveBeenCalled();
    unsubscribe();
    unsubscribe();
    expect(teardown).toHaveBeenCalledTimes(1);
  });

  it("stops delivering after unsubscribe", () => {
    let push;
    const seen = [];
    const unsubscribe = createStream(({ next }) => {
      push = next;
    }).subscribe({ next: (value) => seen.push(value) });
    push(1);
    unsubscribe();
    push(2);
    expect(seen).toEqual([1]);
  });

  it("runs the teardown on complete", () => {
    const teardown = vi.fn();
    createStream(({ complete }) => {
      complete();
      return teardown;
    }).subscribe({});
    expect(teardown).toHaveBeenCalledTimes(1);
  });

  it("drops a next() after complete()", () => {
    const seen = [];
    const late = [];
    createStream(({ next, complete }) => {
      next("before");
      complete();
      next("after");
    }).subscribe({ next: (v) => seen.push(v), complete: () => late.push("done") });
    expect(seen).toEqual(["before"]);
    expect(late).toEqual(["done"]);
  });

  it("delivers an error and then nothing", () => {
    const onError = vi.fn();
    const onNext = vi.fn();
    createStream(({ next, error }) => {
      error(new Error("bad"));
      next("after");
    }).subscribe({ next: onNext, error: onError });
    expect(onError).toHaveBeenCalledTimes(1);
    expect(onError.mock.calls[0][0].message).toBe("bad");
    expect(onNext).not.toHaveBeenCalled();
  });

  it("tolerates an observer missing every callback", () => {
    expect(() =>
      createStream(({ next, complete }) => {
        next(1);
        complete();
      }).subscribe({}),
    ).not.toThrow();
  });

  it("keeps two subscriptions independent", () => {
    const pushes = [];
    const stream = createStream(({ next }) => {
      pushes.push(next);
    });
    const first = [];
    const second = [];
    const unsubscribeFirst = stream.subscribe({ next: (v) => first.push(v) });
    stream.subscribe({ next: (v) => second.push(v) });
    unsubscribeFirst();
    pushes[0]("to first");
    pushes[1]("to second");
    expect(first).toEqual([]);
    expect(second).toEqual(["to second"]);
  });
});

describe("ex080 createSubject", () => {
  it("multicasts to everyone subscribed", () => {
    const subject = createSubject();
    const a = [];
    const b = [];
    subject.subscribe({ next: (v) => a.push(v) });
    subject.subscribe({ next: (v) => b.push(v) });
    subject.next(1);
    expect(a).toEqual([1]);
    expect(b).toEqual([1]);
  });

  it("does not replay what happened before a subscription", () => {
    const subject = createSubject();
    subject.next("missed");
    const seen = [];
    subject.subscribe({ next: (v) => seen.push(v) });
    subject.next("heard");
    expect(seen).toEqual(["heard"]);
  });

  it("unsubscribes one without disturbing the other", () => {
    const subject = createSubject();
    const a = [];
    const b = [];
    const off = subject.subscribe({ next: (v) => a.push(v) });
    subject.subscribe({ next: (v) => b.push(v) });
    off();
    subject.next(1);
    expect(a).toEqual([]);
    expect(b).toEqual([1]);
  });

  it("completes everyone once and then goes quiet", () => {
    const subject = createSubject();
    const onComplete = vi.fn();
    const onNext = vi.fn();
    subject.subscribe({ next: onNext, complete: onComplete });
    subject.complete();
    subject.complete();
    subject.next("after");
    expect(onComplete).toHaveBeenCalledTimes(1);
    expect(onNext).not.toHaveBeenCalled();
  });

  it("survives a subscription after completion", () => {
    const subject = createSubject();
    subject.complete();
    const onNext = vi.fn();
    expect(() => subject.subscribe({ next: onNext })()).not.toThrow();
    subject.next(1);
    expect(onNext).not.toHaveBeenCalled();
  });
});
