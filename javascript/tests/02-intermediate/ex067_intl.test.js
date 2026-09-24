import { describe, expect, it } from "vitest";
import {
  currencyParts,
  dateFields,
  groupSeparator,
  pluralCategory,
  relative,
} from "@ex/02-intermediate/ex067_intl/index.js";

/** The concatenated values of the parts of one type. */
const partsOfType = (parts, type) =>
  parts.filter((part) => part.type === type).map((part) => part.value);

describe("ex067 currencyParts", () => {
  it("labels each piece", () => {
    const parts = currencyParts(1234.5, "en-US", "USD");
    expect(partsOfType(parts, "currency")).toEqual(["$"]);
    expect(partsOfType(parts, "integer")).toEqual(["1", "234"]);
    expect(partsOfType(parts, "fraction")).toEqual(["50"]);
  });

  it("uses the locale's own currency symbol and separators", () => {
    // Measured here: de-CH groups with an apostrophe and writes CHF out in
    // full. Asserting the whole formatted string would also pin the space
    // character ICU chose, which changes between versions.
    const parts = currencyParts(1234.5, "de-CH", "CHF");
    expect(partsOfType(parts, "currency")).toEqual(["CHF"]);
    expect(partsOfType(parts, "group")).toEqual(["'"]);
  });

  it("does not depend on the machine's locale", () => {
    expect(partsOfType(currencyParts(1, "en-US", "EUR"), "currency")).toEqual(["€"]);
  });
});

describe("ex067 groupSeparator", () => {
  it("differs by locale", () => {
    expect(groupSeparator("en-US")).toBe(",");
    expect(groupSeparator("de-DE")).toBe(".");
  });
});

describe("ex067 dateFields", () => {
  it("formats in the requested time zone", () => {
    const instant = new Date("2024-01-31T23:30:00Z");
    expect(dateFields(instant, "en-US", "UTC")).toEqual({
      year: "2024",
      month: "01",
      day: "31",
    });
  });

  it("gives a different day for a time zone that has already turned over", () => {
    // Same instant, one hour ahead of UTC: 00:30 on the 1st.
    const instant = new Date("2024-01-31T23:30:00Z");
    expect(dateFields(instant, "de-DE", "Europe/Zurich")).toEqual({
      year: "2024",
      month: "02",
      day: "01",
    });
  });

  it("pads to two digits", () => {
    expect(dateFields(new Date("2024-03-05T12:00:00Z"), "en-US", "UTC")).toEqual({
      year: "2024",
      month: "03",
      day: "05",
    });
  });
});

describe("ex067 relative", () => {
  it("formats future and past", () => {
    expect(relative(3, "day", "en")).toBe("in 3 days");
    expect(relative(-3, "day", "en")).toBe("3 days ago");
  });

  it("translates", () => {
    expect(relative(3, "day", "de")).toBe("in 3 Tagen");
  });

  it("keeps the number even for 1, because numeric is always", () => {
    // With numeric: "auto" this would be "tomorrow".
    expect(relative(1, "day", "en")).toBe("in 1 day");
  });
});

describe("ex067 pluralCategory", () => {
  it("uses English's two categories", () => {
    expect(pluralCategory(1, "en")).toBe("one");
    expect(pluralCategory(0, "en")).toBe("other");
    expect(pluralCategory(2, "en")).toBe("other");
  });

  it("uses each locale's own rules", () => {
    // French counts 0 as singular; English does not.
    expect(pluralCategory(0, "fr")).toBe("one");
  });
});
