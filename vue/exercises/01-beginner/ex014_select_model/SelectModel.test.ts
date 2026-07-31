import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import SelectModel from "./SelectModel.vue";

describe("SelectModel", () => {
  it("starts with the first option selected", () => {
    const wrapper = mount(SelectModel);
    const select = wrapper.get("[data-testid='fruit-select']")
      .element as HTMLSelectElement;

    expect(select.value).toBe("apple");
    expect(wrapper.get("[data-testid='status']").text()).toBe(
      "Selected: apple",
    );
  });

  it("updates `selected` when a different option is chosen", async () => {
    const wrapper = mount(SelectModel);
    const select = wrapper.get("[data-testid='fruit-select']");

    await select.setValue("cherry");

    expect(wrapper.get("[data-testid='status']").text()).toBe(
      "Selected: cherry",
    );
  });

  it("reflects a further change back to another option", async () => {
    const wrapper = mount(SelectModel);
    const select = wrapper.get("[data-testid='fruit-select']");

    await select.setValue("banana");
    expect(wrapper.get("[data-testid='status']").text()).toBe(
      "Selected: banana",
    );

    await select.setValue("cherry");
    expect(wrapper.get("[data-testid='status']").text()).toBe(
      "Selected: cherry",
    );
  });
});
