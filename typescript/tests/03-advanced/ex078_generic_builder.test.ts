import { describe, expect, it } from "vitest";
import { configBuilder } from "@ex/03-advanced/ex078_generic_builder/index";

describe("ex078 configBuilder", () => {
  it("builds a complete config", () => {
    const config = configBuilder()
      .set("host", "localhost")
      .set("port", 8080)
      .set("secure", true)
      .build();
    expect(config).toEqual({ host: "localhost", port: 8080, secure: true });
  });

  it("takes the last value for a repeated key", () => {
    const config = configBuilder()
      .set("host", "a")
      .set("host", "b")
      .set("port", 1)
      .set("secure", false)
      .build();
    expect(config.host).toBe("b");
  });

  it("returns a fresh object rather than its own state", () => {
    const builder = configBuilder().set("host", "a").set("port", 1).set("secure", false);
    const first = builder.build();
    const second = builder.build();
    expect(first).toEqual(second);
    expect(first).not.toBe(second);
  });
});
