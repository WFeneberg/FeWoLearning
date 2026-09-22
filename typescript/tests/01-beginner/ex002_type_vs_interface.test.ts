import { describe, expect, it } from "vitest";
import { formatConfig } from "@ex/01-beginner/ex002_type_vs_interface/index";

describe("ex002 formatConfig", () => {
  it("renders host and port", () => {
    // Inferred local rather than a fresh literal at the call site: a fresh
    // object literal is subject to the excess-property check, and before the
    // second declaration merges in, `port` would be flagged as excess.
    const config = { host: "localhost", port: 8080 };
    expect(formatConfig(config)).toBe("localhost:8080");
  });

  it("renders a different pair", () => {
    const config = { host: "db.internal", port: 5432 };
    expect(formatConfig(config)).toBe("db.internal:5432");
  });
});
