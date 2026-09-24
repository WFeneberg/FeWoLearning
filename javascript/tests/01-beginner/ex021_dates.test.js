import { describe, expect, it } from "vitest";
import {
  addDays,
  daysBetween,
  startOfUtcDay,
  toIsoDate,
} from "@ex/01-beginner/ex021_dates/index.js";

describe("ex021 toIsoDate", () => {
  it("formats in UTC", () => {
    expect(toIsoDate(new Date("2024-01-31T12:00:00.000Z"))).toBe("2024-01-31");
  });

  it("uses the UTC day, not the local one", () => {
    // 23:30 UTC is already the next day in Zurich. The answer must not
    // depend on where this test runs.
    expect(toIsoDate(new Date("2024-01-31T23:30:00.000Z"))).toBe("2024-01-31");
  });

  it("agrees with the way a date-only string is parsed", () => {
    // "2024-01-31" is parsed as UTC midnight; "2024-01-31T00:00:00" (no Z)
    // would be LOCAL midnight. One character, two different instants.
    expect(new Date("2024-01-31").toISOString()).toBe("2024-01-31T00:00:00.000Z");
    expect(toIsoDate(new Date("2024-01-31"))).toBe("2024-01-31");
  });
});

describe("ex021 addDays", () => {
  it("moves forward and back", () => {
    const start = new Date("2024-01-31T12:00:00.000Z");
    expect(toIsoDate(addDays(start, 1))).toBe("2024-02-01");
    expect(toIsoDate(addDays(start, -1))).toBe("2024-01-30");
  });

  it("crosses a leap day", () => {
    expect(toIsoDate(addDays(new Date("2024-02-28T10:00:00.000Z"), 2))).toBe("2024-03-01");
  });

  it("keeps the time of day", () => {
    expect(addDays(new Date("2024-01-31T12:34:56.789Z"), 3).toISOString()).toBe(
      "2024-02-03T12:34:56.789Z",
    );
  });

  it("does not mutate the Date it was given", () => {
    // setDate() would. Date is the one mutable built-in people forget about.
    const start = new Date("2024-01-31T12:00:00.000Z");
    const moved = addDays(start, 5);
    expect(start.toISOString()).toBe("2024-01-31T12:00:00.000Z");
    expect(moved).not.toBe(start);
  });
});

describe("ex021 daysBetween", () => {
  it("counts whole days", () => {
    expect(daysBetween(new Date("2024-01-01T00:00:00Z"), new Date("2024-01-04T00:00:00Z"))).toBe(3);
  });

  it("counts calendar days, not 24-hour blocks", () => {
    // Two hours apart, but on different days.
    expect(daysBetween(new Date("2024-01-01T23:00:00Z"), new Date("2024-01-02T01:00:00Z"))).toBe(1);
  });

  it("is negative going backwards, and 0 within one day", () => {
    expect(daysBetween(new Date("2024-01-04T00:00:00Z"), new Date("2024-01-01T00:00:00Z"))).toBe(-3);
    expect(daysBetween(new Date("2024-01-01T01:00:00Z"), new Date("2024-01-01T23:00:00Z"))).toBe(0);
  });

  it("survives the spring-forward weekend, where a local-time subtraction gives 0.958", () => {
    expect(daysBetween(new Date("2024-03-30T12:00:00Z"), new Date("2024-03-31T12:00:00Z"))).toBe(1);
  });
});

describe("ex021 startOfUtcDay", () => {
  it("zeroes the time", () => {
    expect(startOfUtcDay(new Date("2024-01-31T23:59:59.999Z")).toISOString()).toBe(
      "2024-01-31T00:00:00.000Z",
    );
  });

  it("returns a new Date", () => {
    const date = new Date("2024-01-31T10:00:00Z");
    expect(startOfUtcDay(date)).not.toBe(date);
    expect(date.toISOString()).toBe("2024-01-31T10:00:00.000Z");
  });
});
