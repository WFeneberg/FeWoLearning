import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RadioGroupModel from "./RadioGroupModel.vue";

describe("RadioGroupModel", () => {
  it("starts with apple selected", () => {
    const wrapper = mount(RadioGroupModel);
    const apple = wrapper.get("[data-testid='radio-apple']")
      .element as HTMLInputElement;

    expect(apple.checked).toBe(true);
    expect(wrapper.get("[data-testid='choice']").text()).toBe("apple");
  });

  it("sets choice to banana when the banana radio is selected", async () => {
    const wrapper = mount(RadioGroupModel);
    const banana = wrapper.get("[data-testid='radio-banana']");

    await banana.setValue(true);

    expect(wrapper.get("[data-testid='choice']").text()).toBe("banana");
  });

  it("only keeps one radio checked at a time", async () => {
    const wrapper = mount(RadioGroupModel);
    const banana = wrapper.get("[data-testid='radio-banana']");
    const cherry = wrapper.get("[data-testid='radio-cherry']");

    await banana.setValue(true);
    await cherry.setValue(true);

    const bananaEl = banana.element as HTMLInputElement;
    const cherryEl = cherry.element as HTMLInputElement;

    expect(bananaEl.checked).toBe(false);
    expect(cherryEl.checked).toBe(true);
    expect(wrapper.get("[data-testid='choice']").text()).toBe("cherry");
  });
});
