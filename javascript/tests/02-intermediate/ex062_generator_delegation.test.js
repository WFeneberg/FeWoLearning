import { describe, expect, it } from "vitest";
import {
  countThenTotal,
  echo,
  flatten,
  yieldAndSum,
} from "@ex/02-intermediate/ex062_generator_delegation/index.js";

describe("ex062 flatten", () => {
  it("walks a nested structure depth-first", () => {
    expect([...flatten([1, [2, [3]], 4])]).toEqual([1, 2, 3, 4]);
  });

  it("handles a bare value and an empty array", () => {
    expect([...flatten(5)]).toEqual([5]);
    expect([...flatten([])]).toEqual([]);
    expect([...flatten([[], [[]]])]).toEqual([]);
  });

  it("stays lazy", () => {
    const iterator = flatten([1, [2, 3]]);
    expect(iterator.next().value).toBe(1);
    expect(iterator.next().value).toBe(2);
  });
});

describe("ex062 yieldAndSum", () => {
  it("yields the values", () => {
    expect([...yieldAndSum([1, 2, 3])]).toEqual([1, 2, 3]);
  });

  it("returns the total, which spreading throws away", () => {
    const iterator = yieldAndSum([1, 2]);
    expect(iterator.next()).toEqual({ value: 1, done: false });
    expect(iterator.next()).toEqual({ value: 2, done: false });
    expect(iterator.next()).toEqual({ value: 3, done: true });
  });

  it("returns 0 for no values", () => {
    expect(yieldAndSum([]).next()).toEqual({ value: 0, done: true });
  });
});

describe("ex062 countThenTotal", () => {
  it("passes the delegate's yields through and then its return value", () => {
    expect([...countThenTotal([1, 2, 3])]).toEqual([1, 2, 3, "total: 6"]);
  });

  it("works for an empty list", () => {
    expect([...countThenTotal([])]).toEqual(["total: 0"]);
  });
});

describe("ex062 echo", () => {
  it("receives what next() sends", () => {
    const conversation = echo();
    expect(conversation.next().value).toBe("ready");
    expect(conversation.next("a").value).toBe("echo: a");
    expect(conversation.next("b").value).toBe("echo: b");
    expect(conversation.next("done")).toEqual({ value: ["a", "b"], done: true });
  });

  it("ignores the value passed to the FIRST next(), which has nowhere to land", () => {
    // There is no suspended yield to receive it yet.
    const conversation = echo();
    expect(conversation.next("ignored").value).toBe("ready");
    expect(conversation.next("done").value).toEqual([]);
  });

  it("collects nothing when it is stopped immediately", () => {
    const conversation = echo();
    conversation.next();
    expect(conversation.next("done")).toEqual({ value: [], done: true });
  });
});
