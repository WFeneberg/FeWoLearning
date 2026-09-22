import { describe, expect, it } from "vitest";
import { Temperature } from "@ex/01-beginner/ex024_accessors_and_static/index";

describe("ex024 the constants", () => {
  it("is absolute zero in Celsius", () => {
    expect(Temperature.ABSOLUTE_ZERO_C).toBe(-273.15);
  });

  it("derives the same point in Fahrenheit", () => {
    expect(Temperature.ABSOLUTE_ZERO_F).toBeCloseTo(-459.67, 10);
  });
});

describe("ex024 celsius", () => {
  it("starts at zero", () => {
    expect(new Temperature().celsius).toBe(0);
  });

  it("round-trips a value through the accessor pair", () => {
    const temperature = new Temperature();
    temperature.celsius = 25;
    expect(temperature.celsius).toBe(25);
  });

  it("clamps a value below absolute zero", () => {
    const temperature = new Temperature();
    temperature.celsius = -300;
    expect(temperature.celsius).toBe(-273.15);
  });

  it("accepts absolute zero itself", () => {
    const temperature = new Temperature();
    temperature.celsius = -273.15;
    expect(temperature.celsius).toBe(-273.15);
  });
});

describe("ex024 fahrenheit", () => {
  it("derives from the stored Celsius value", () => {
    const temperature = new Temperature();
    temperature.celsius = 100;
    expect(temperature.fahrenheit).toBe(212);
  });

  it("tracks a later write to celsius", () => {
    const temperature = new Temperature();
    temperature.celsius = 0;
    expect(temperature.fahrenheit).toBe(32);
    temperature.celsius = 37;
    expect(temperature.fahrenheit).toBeCloseTo(98.6, 10);
  });
});

describe("ex024 fromFahrenheit", () => {
  it("builds a Temperature from a Fahrenheit reading", () => {
    expect(Temperature.fromFahrenheit(212).celsius).toBeCloseTo(100, 10);
  });

  it("round-trips through both directions", () => {
    expect(Temperature.fromFahrenheit(98.6).fahrenheit).toBeCloseTo(98.6, 10);
  });
});
