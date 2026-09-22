import { describe, expect, it } from "vitest";
import { reduce } from "@ex/02-intermediate/ex054_typed_reducer/index";
import type { Action, State } from "@ex/02-intermediate/ex054_typed_reducer/index";

const start: State = { items: ["a"], total: 10 };

describe("ex054 reduce", () => {
  it("appends on add and increases the total", () => {
    expect(reduce(start, { type: "add", item: "b", amount: 5 } as Action)).toEqual({
      items: ["a", "b"],
      total: 15,
    });
  });

  it("drops every copy on remove and leaves the total", () => {
    const state: State = { items: ["a", "b", "a"], total: 10 };
    expect(reduce(state, { type: "remove", item: "a" } as Action)).toEqual({
      items: ["b"],
      total: 10,
    });
  });

  it("empties everything on clear", () => {
    expect(reduce(start, { type: "clear" } as Action)).toEqual({ items: [], total: 0 });
  });

  it("does not mutate the state it was given", () => {
    reduce(start, { type: "add", item: "b", amount: 5 } as Action);
    expect(start).toEqual({ items: ["a"], total: 10 });
  });

  it("removing something absent changes nothing", () => {
    expect(reduce(start, { type: "remove", item: "zz" } as Action)).toEqual({
      items: ["a"],
      total: 10,
    });
  });
});
