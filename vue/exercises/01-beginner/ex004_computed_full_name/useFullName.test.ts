import { describe, expect, it } from "vitest";
import { useFullName } from "./useFullName";

describe("useFullName", () => {
  it("combines the initial first and last name", () => {
    const { fullName } = useFullName("Ada", "Lovelace");
    expect(fullName.value).toBe("Ada Lovelace");
  });

  it("updates when firstName changes", () => {
    const { firstName, fullName } = useFullName("Ada", "Lovelace");
    firstName.value = "Grace";
    expect(fullName.value).toBe("Grace Lovelace");
  });

  it("updates when lastName changes", () => {
    const { lastName, fullName } = useFullName("Ada", "Lovelace");
    lastName.value = "Hopper";
    expect(fullName.value).toBe("Ada Hopper");
  });
});
