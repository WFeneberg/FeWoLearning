import { describe, expect, it } from "vitest";
import {
  cleaningProducer,
  pullThenReturn,
  resilient,
  throwInto,
} from "@ex/03-advanced/ex083_generator_cancellation/index.js";

describe("ex083 cleaningProducer", () => {
  it("yields its values and cleans up at the end", () => {
    const log = [];
    expect([...cleaningProducer([1, 2], log)]).toEqual([1, 2]);
    expect(log).toEqual(["cleanup"]);
  });

  it("cleans up when the consumer breaks out early", () => {
    const log = [];
    for (const value of cleaningProducer([1, 2, 3], log)) {
      if (value === 2) break;
    }
    expect(log).toEqual(["cleanup"]);
  });

  it("does not clean up while it is still suspended", () => {
    const log = [];
    const producer = cleaningProducer([1, 2, 3], log);
    producer.next();
    expect(log).toEqual([]);
  });

  it("cleans up when the consumer throws", () => {
    const log = [];
    expect(() => {
      for (const value of cleaningProducer([1, 2], log)) {
        throw new Error(`stopped at ${value}`);
      }
    }).toThrow();
    expect(log).toEqual(["cleanup"]);
  });
});

describe("ex083 pullThenReturn", () => {
  it("takes what it asked for and then stops the generator", () => {
    const log = [];
    const { values, result } = pullThenReturn(cleaningProducer([1, 2, 3], log), 2);
    expect(values).toEqual([1, 2]);
    expect(result).toEqual({ value: "stopped", done: true });
    expect(log).toEqual(["cleanup"]);
  });

  it("leaves the generator finished", () => {
    const generator = cleaningProducer([1, 2, 3], []);
    pullThenReturn(generator, 1);
    expect(generator.next()).toEqual({ value: undefined, done: true });
  });

  it("copes with a generator that ends first", () => {
    const { values } = pullThenReturn(cleaningProducer([1], []), 5);
    expect(values).toEqual([1]);
  });
});

describe("ex083 resilient", () => {
  it("counts up", () => {
    const generator = resilient();
    expect(generator.next().value).toBe(1);
    expect(generator.next().value).toBe(2);
  });

  it("catches an injected error and carries on", () => {
    const generator = resilient();
    generator.next();
    expect(throwInto(generator, new Error("interrupted"))).toEqual({
      yielded: "caught: interrupted",
    });
    expect(generator.next().value).toBe(2);
    expect(generator.next().done).toBe(false);
  });
});

describe("ex083 throwInto", () => {
  it("reports an error the generator does not catch", () => {
    const generator = cleaningProducer([1, 2, 3], []);
    generator.next();
    expect(throwInto(generator, new Error("boom"))).toBe("propagated");
  });

  it("still runs the producer's finally on the way out", () => {
    const log = [];
    const generator = cleaningProducer([1, 2], log);
    generator.next();
    throwInto(generator, new Error("boom"));
    expect(log).toEqual(["cleanup"]);
  });

  it("reports what a catching generator yielded instead", () => {
    const generator = resilient();
    generator.next();
    expect(throwInto(generator, new Error("handled"))).toEqual({
      yielded: "caught: handled",
    });
  });
});
