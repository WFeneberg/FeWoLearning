import { ref } from "vue";
import { describe, expect, it } from "vitest";
import { minLength, required, useValidation } from "./useValidation";

describe("useValidation", () => {
  it("reports no errors when all rules pass", () => {
    const value = ref("hello");
    const { errors, isValid } = useValidation(value, [required, minLength(3)]);
    expect(errors.value).toEqual([]);
    expect(isValid.value).toBe(true);
  });

  it("collects an error message for each failing rule", () => {
    const value = ref("");
    const { errors, isValid } = useValidation(value, [required, minLength(3)]);
    expect(errors.value).toEqual([
      "This field is required.",
      "Must be at least 3 characters.",
    ]);
    expect(isValid.value).toBe(false);
  });

  it("re-evaluates reactively when the value changes", () => {
    const value = ref("ab");
    const { errors, isValid } = useValidation(value, [required, minLength(3)]);
    expect(errors.value).toEqual(["Must be at least 3 characters."]);
    expect(isValid.value).toBe(false);

    value.value = "abc";
    expect(errors.value).toEqual([]);
    expect(isValid.value).toBe(true);
  });

  it("supports a custom rule with its own error message", () => {
    const noDigits = (v: string) =>
      /\d/.test(v) ? "Digits are not allowed." : null;
    const value = ref("abc123");
    const { errors } = useValidation(value, [noDigits]);
    expect(errors.value).toEqual(["Digits are not allowed."]);
  });
});
