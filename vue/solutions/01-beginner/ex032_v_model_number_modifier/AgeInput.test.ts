import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import AgeInput from "./AgeInput.vue";

describe("AgeInput", () => {
  it("coerces the input's string value into a number on age", async () => {
    const wrapper = mount(AgeInput);
    const input = wrapper.get<HTMLInputElement>('[data-testid="age-input"]');

    await input.setValue("42");

    const age = (wrapper.vm as unknown as { age: number }).age;
    expect(age).toBe(42);
    expect(typeof age).toBe("number");
  });

  it("keeps age as a number even for decimal strings", async () => {
    const wrapper = mount(AgeInput);
    const input = wrapper.get<HTMLInputElement>('[data-testid="age-input"]');

    await input.setValue("7.5");

    const age = (wrapper.vm as unknown as { age: number }).age;
    expect(age).toBe(7.5);
    expect(typeof age).toBe("number");
  });
});
