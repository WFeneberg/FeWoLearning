import { describe, expect, it } from "vitest";
import { className } from "@ex/02-intermediate/ex047_template_literal_types/index";

describe("ex047 className", () => {
  it("joins the variant and the size", () => {
    expect(className("primary", "sm")).toBe("primary-sm");
  });

  it("works for another combination", () => {
    expect(className("ghost", "lg")).toBe("ghost-lg");
  });
});
