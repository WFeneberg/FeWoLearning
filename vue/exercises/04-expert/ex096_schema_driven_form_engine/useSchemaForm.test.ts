import { describe, expect, it, vi } from "vitest";
import { useSchemaForm, type FieldSchema } from "./useSchemaForm";

const schema: FieldSchema[] = [
  { name: "username", type: "text", required: true, min: 3, max: 8 },
  { name: "age", type: "number", min: 18, max: 120 },
  { name: "terms", type: "checkbox", required: true },
];

describe("useSchemaForm", () => {
  it("derives initial values from each field's type", () => {
    const form = useSchemaForm(schema);

    expect(form.values).toEqual({ username: "", age: 0, terms: false });
  });

  it("reports required errors for an empty text field and a false checkbox", () => {
    const form = useSchemaForm(schema);

    expect(form.errors.value.username).toBe("is required");
    expect(form.errors.value.terms).toBe("is required");
    expect(form.isValid.value).toBe(false);
  });

  it("checks min/max as a length for text", () => {
    const form = useSchemaForm(schema);

    form.values.username = "ab";
    expect(form.errors.value.username).toBe("must be at least 3 characters");

    form.values.username = "abcdefghij";
    expect(form.errors.value.username).toBe("must be at most 8 characters");

    form.values.username = "abcd";
    expect(form.errors.value.username).toBeUndefined();
  });

  it("checks min/max as a value for numbers", () => {
    const form = useSchemaForm(schema);

    form.values.age = 17;
    expect(form.errors.value.age).toBe("must be at least 18");

    form.values.age = 500;
    expect(form.errors.value.age).toBe("must be at most 120");

    form.values.age = 30;
    expect(form.errors.value.age).toBeUndefined();
  });

  it("runs a field's own validate callback after the built-in rules", () => {
    const form = useSchemaForm([
      {
        name: "code",
        type: "text",
        required: true,
        min: 2,
        validate: (v) => (String(v).startsWith("X") ? null : "must start with X"),
      },
    ]);

    // required wins over the callback while the field is empty
    expect(form.errors.value.code).toBe("is required");

    form.values.code = "a";
    // min wins over the callback
    expect(form.errors.value.code).toBe("must be at least 2 characters");

    form.values.code = "ab";
    expect(form.errors.value.code).toBe("must start with X");

    form.values.code = "Xb";
    expect(form.errors.value.code).toBeUndefined();
  });

  it("becomes valid once every rule is satisfied", () => {
    const form = useSchemaForm(schema);

    form.values.username = "wolf";
    form.values.age = 42;
    form.values.terms = true;

    expect(form.errors.value).toEqual({});
    expect(form.isValid.value).toBe(true);
  });

  it("submit refuses to run while the form is invalid", () => {
    const form = useSchemaForm(schema);
    const onSubmit = vi.fn();

    expect(form.submit(onSubmit)).toBe(false);
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("submit hands over a plain snapshot when valid", () => {
    const form = useSchemaForm(schema);
    const onSubmit = vi.fn();

    form.values.username = "wolf";
    form.values.age = 42;
    form.values.terms = true;

    expect(form.submit(onSubmit)).toBe(true);
    expect(onSubmit).toHaveBeenCalledTimes(1);
    expect(onSubmit).toHaveBeenCalledWith({ username: "wolf", age: 42, terms: true });

    // A snapshot, not the live reactive object: later edits must not leak into it.
    const passed = onSubmit.mock.calls[0][0] as Record<string, unknown>;
    form.values.username = "changed";
    expect(passed.username).toBe("wolf");
  });

  it("reset restores the type-derived initial values", () => {
    const form = useSchemaForm(schema);

    form.values.username = "wolf";
    form.values.age = 42;
    form.values.terms = true;

    form.reset();

    expect(form.values).toEqual({ username: "", age: 0, terms: false });
    expect(form.isValid.value).toBe(false);
  });
});
