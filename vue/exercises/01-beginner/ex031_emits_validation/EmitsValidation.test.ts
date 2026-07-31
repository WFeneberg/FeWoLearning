import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import EmitsValidation from "./EmitsValidation.vue";

describe("EmitsValidation", () => {
  const validator = (EmitsValidation as any).emits.change as (payload: unknown) => boolean;

  it("validator returns true for numeric payloads", () => {
    expect(validator(42)).toBe(true);
    expect(validator(0)).toBe(true);
    expect(validator(-3.5)).toBe(true);
  });

  it("validator returns false for non-numeric payloads", () => {
    expect(validator("not-a-number")).toBe(false);
    expect(validator(null)).toBe(false);
    expect(validator(undefined)).toBe(false);
    expect(validator({})).toBe(false);
  });

  it("emits 'change' with a numeric payload when the valid button is clicked", async () => {
    const wrapper = mount(EmitsValidation);
    await wrapper.find('[data-testid="valid"]').trigger("click");

    expect(wrapper.emitted("change")).toEqual([[42]]);
  });

  it("emits 'change' with a non-numeric payload when the invalid button is clicked", async () => {
    const wrapper = mount(EmitsValidation);
    await wrapper.find('[data-testid="invalid"]').trigger("click");

    expect(wrapper.emitted("change")).toEqual([["not-a-number"]]);
  });
});
