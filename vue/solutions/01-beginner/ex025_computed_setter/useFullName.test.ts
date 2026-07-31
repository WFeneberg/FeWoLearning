import { describe, expect, it } from "vitest";
import { useFullName } from "./useFullName";

describe("useFullName", () => {
  it("joins firstName and lastName into fullName", () => {
    const { fullName } = useFullName("Ada", "Lovelace");
    expect(fullName.value).toBe("Ada Lovelace");
  });

  it("updates fullName when firstName or lastName change", () => {
    const { firstName, lastName, fullName } = useFullName("Ada", "Lovelace");
    firstName.value = "Grace";
    lastName.value = "Hopper";
    expect(fullName.value).toBe("Grace Hopper");
  });

  it("splits an assigned fullName into firstName and lastName", () => {
    const { firstName, lastName, fullName } = useFullName("Ada", "Lovelace");
    fullName.value = "Alan Turing";
    expect(firstName.value).toBe("Alan");
    expect(lastName.value).toBe("Turing");
  });
});
