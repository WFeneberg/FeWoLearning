import { describe, expect, it } from "vitest";
import { makeProbe, makeTemperature } from "@ex/01-beginner/ex024_getters_and_setters/index.js";

describe("ex024 makeTemperature", () => {
  it("converts on read", () => {
    expect(makeTemperature(0).fahrenheit).toBe(32);
    expect(makeTemperature(100).fahrenheit).toBe(212);
    expect(makeTemperature(-40).fahrenheit).toBe(-40);
  });

  it("stays live — a getter computes, it does not snapshot", () => {
    const temp = makeTemperature(0);
    temp.celsius = 100;
    expect(temp.fahrenheit).toBe(212);
  });

  it("converts on write, in the other direction", () => {
    const temp = makeTemperature(0);
    temp.fahrenheit = 212;
    expect(temp.celsius).toBe(100);
  });

  it("is an accessor, not a value computed once at construction", () => {
    // The descriptor is the mechanism check: a data property would have a
    // `value` and no `get`.
    const descriptor = Object.getOwnPropertyDescriptor(makeTemperature(0), "fahrenheit");
    expect(typeof descriptor.get).toBe("function");
    expect(descriptor.value).toBeUndefined();
  });

  it("validates in both setters and at construction", () => {
    expect(() => makeTemperature(-300)).toThrow(RangeError);
    expect(() => makeTemperature(-300)).toThrow("below absolute zero");

    const temp = makeTemperature(0);
    expect(() => {
      temp.celsius = -274;
    }).toThrow(RangeError);
    expect(() => {
      temp.fahrenheit = -500;
    }).toThrow(RangeError);
  });

  it("keeps the old value after a rejected write", () => {
    const temp = makeTemperature(20);
    try {
      temp.celsius = -300;
    } catch {
      // expected
    }
    expect(temp.celsius).toBe(20);
  });

  it("accepts exactly absolute zero", () => {
    expect(makeTemperature(-273.15).celsius).toBe(-273.15);
  });

  it("serializes getters like ordinary properties", () => {
    expect(JSON.parse(JSON.stringify(makeTemperature(25)))).toEqual({
      celsius: 25,
      fahrenheit: 77,
    });
  });
});

describe("ex024 makeProbe", () => {
  it("counts reads, because a getter is a call", () => {
    const probe = makeProbe();
    expect(probe.callCount).toBe(0);
    void probe.value;
    void probe.value;
    expect(probe.callCount).toBe(2);
  });

  it("returns the value each time", () => {
    expect(makeProbe().value).toBe("read");
  });
});
