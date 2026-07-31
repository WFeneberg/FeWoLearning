import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import CheckboxModel from "./CheckboxModel.vue";

describe("CheckboxModel", () => {
  it("starts unchecked with a not-agreed status", () => {
    const wrapper = mount(CheckboxModel);
    const checkbox = wrapper.get("[data-testid='agreed-checkbox']")
      .element as HTMLInputElement;

    expect(checkbox.checked).toBe(false);
    expect(wrapper.get("[data-testid='status']").text()).toBe("Not agreed");
  });

  it("becomes agreed when the checkbox is checked", async () => {
    const wrapper = mount(CheckboxModel);
    const checkbox = wrapper.get("[data-testid='agreed-checkbox']");

    await checkbox.setValue(true);

    expect(wrapper.get("[data-testid='status']").text()).toBe("Agreed");
  });

  it("toggles back to not agreed when unchecked again", async () => {
    const wrapper = mount(CheckboxModel);
    const checkbox = wrapper.get("[data-testid='agreed-checkbox']");

    await checkbox.setValue(true);
    expect(wrapper.get("[data-testid='status']").text()).toBe("Agreed");

    await checkbox.setValue(false);
    expect(wrapper.get("[data-testid='status']").text()).toBe("Not agreed");
  });
});
