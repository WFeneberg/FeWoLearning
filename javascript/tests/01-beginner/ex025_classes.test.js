import { describe, expect, it } from "vitest";
import { Stack } from "@ex/01-beginner/ex025_classes/index.js";

describe("ex025 Stack", () => {
  it("pushes, peeks and pops in LIFO order", () => {
    const stack = new Stack();
    stack.push(1).push(2);
    expect(stack.peek()).toBe(2);
    expect(stack.pop()).toBe(2);
    expect(stack.pop()).toBe(1);
    expect(stack.pop()).toBeUndefined();
  });

  it("chains pushes by returning this", () => {
    const stack = new Stack();
    expect(stack.push("a")).toBe(stack);
  });

  it("reports size as a getter, not a method", () => {
    const stack = Stack.from([1, 2, 3]);
    expect(stack.size).toBe(3);
    const descriptor = Object.getOwnPropertyDescriptor(Stack.prototype, "size");
    expect(typeof descriptor.get).toBe("function");
  });

  it("peek leaves the stack alone; pop does not", () => {
    const stack = Stack.from(["a", "b"]);
    stack.peek();
    expect(stack.size).toBe(2);
    stack.pop();
    expect(stack.size).toBe(1);
  });

  it("builds from any iterable, in iteration order", () => {
    expect(Stack.from("ab").pop()).toBe("b");
    expect(Stack.from(new Set([1, 2])).pop()).toBe(2);
    expect(Stack.from([]).size).toBe(0);
  });

  it("gives each instance its own items", () => {
    const first = new Stack();
    const second = new Stack();
    first.push(1);
    expect(second.size).toBe(0);
  });
});

describe("ex025 what the class syntax builds", () => {
  it("puts the methods on the prototype, not on the instance", () => {
    const stack = Stack.from([1]);
    expect(Object.keys(stack)).toEqual(["items"]);
    expect(Object.hasOwn(stack, "push")).toBe(false);
    expect(Object.hasOwn(Stack.prototype, "push")).toBe(true);
  });

  it("makes those methods non-enumerable, so for..in skips them", () => {
    const seen = [];
    for (const key in Stack.from([1])) seen.push(key);
    expect(seen).toEqual(["items"]);
  });

  it("shares one function object across instances", () => {
    expect(new Stack().push).toBe(new Stack().push);
  });

  it("refuses to be called without new", () => {
    // Anchored: a class refuses this from the moment it is written, so on
    // its own the fact would be green against the untouched stub.
    expect(new Stack().size).toBe(0);
    expect(() => Stack()).toThrow(TypeError);
  });

  it("answers instanceof", () => {
    expect(new Stack()).toBeInstanceOf(Stack);
    expect(Object.getPrototypeOf(new Stack())).toBe(Stack.prototype);
  });

  it("keeps the static off the instances", () => {
    expect(typeof Stack.from).toBe("function");
    expect(new Stack().from).toBeUndefined();
  });
});
