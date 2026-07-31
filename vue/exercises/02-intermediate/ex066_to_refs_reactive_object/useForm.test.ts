import { describe, expect, it } from "vitest";
import { useForm } from "./useForm";

describe("useForm", () => {
  it("exposes the initial values through both state and fields", () => {
    const { state, fields } = useForm({ name: "Ada", email: "ada@example.com" });
    expect(state.name).toBe("Ada");
    expect(fields.name.value).toBe("Ada");
    expect(fields.email.value).toBe("ada@example.com");
  });

  it("mutating a destructured ref updates the underlying reactive object", () => {
    const { state, fields } = useForm({ name: "Ada", email: "ada@example.com" });
    fields.name.value = "Grace";
    expect(state.name).toBe("Grace");
    expect(fields.email.value).toBe("ada@example.com");
  });

  it("mutating the reactive object updates the destructured ref", () => {
    const { state, fields } = useForm({ name: "Ada", email: "ada@example.com" });
    state.email = "grace@example.com";
    expect(fields.email.value).toBe("grace@example.com");
    expect(fields.name.value).toBe("Ada");
  });

  it("keeps fields independent per call", () => {
    const first = useForm({ name: "Ada", email: "a@example.com" });
    const second = useForm({ name: "Grace", email: "g@example.com" });
    first.fields.name.value = "Changed";
    expect(second.fields.name.value).toBe("Grace");
  });
});
