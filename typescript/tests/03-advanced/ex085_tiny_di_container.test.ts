import { describe, expect, it } from "vitest";
import { Container, token } from "@ex/03-advanced/ex085_tiny_di_container/index";

interface Clock {
  now(): number;
}

// Built inside each test, not at module level: the untouched stub's
// token() throws, and a throw during module evaluation takes the whole
// file down — reporting 0 tests rather than 8 failures.
const clockToken = (): ReturnType<typeof token<Clock>> => token<Clock>("Clock");
const greetingToken = (): ReturnType<typeof token<string>> => token<string>("Greeting");

describe("ex085 token", () => {
  it("carries only its name at runtime", () => {
    expect(token<number>("Answer")).toEqual({ name: "Answer" });
  });
});

describe("ex085 register", () => {
  it("resolves what the factory produces", () => {
    const container = new Container().register(greetingToken(), () => "hello");
    expect(container.resolve(greetingToken())).toBe("hello");
  });

  it("runs the factory on every resolve", () => {
    let calls = 0;
    const container = new Container().register(greetingToken(), () => `call-${++calls}`);
    expect([container.resolve(greetingToken()), container.resolve(greetingToken())]).toEqual([
      "call-1",
      "call-2",
    ]);
  });

  it("returns the container, so registration chains", () => {
    const container = new Container()
      .register(greetingToken(), () => "hi")
      .register(clockToken(), () => ({ now: () => 1 }));
    expect(container.resolve(greetingToken())).toBe("hi");
    expect((container.resolve(clockToken()) as Clock).now()).toBe(1);
  });
});

describe("ex085 registerSingleton", () => {
  it("runs the factory once, however often it is resolved", () => {
    let calls = 0;
    const container = new Container().registerSingleton(clockToken(), () => {
      calls += 1;
      return { now: () => 42 };
    });
    const first = container.resolve(clockToken());
    const second = container.resolve(clockToken());
    expect(calls).toBe(1);
    expect(first).toBe(second);
  });

  it("does not run the factory until something asks", () => {
    let calls = 0;
    new Container().registerSingleton(clockToken(), () => {
      calls += 1;
      return { now: () => 0 };
    });
    expect(calls).toBe(0);
  });

  it("keeps different tokens apart", () => {
    const container = new Container()
      .registerSingleton(greetingToken(), () => "once")
      .registerSingleton(clockToken(), () => ({ now: () => 7 }));
    expect(container.resolve(greetingToken())).toBe("once");
    expect((container.resolve(clockToken()) as Clock).now()).toBe(7);
  });
});

describe("ex085 resolve", () => {
  it("throws for an unregistered token, naming it", () => {
    expect(() => new Container().resolve(clockToken())).toThrow(/Clock/);
  });
});
