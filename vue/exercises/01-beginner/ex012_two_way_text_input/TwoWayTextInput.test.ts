import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import TwoWayTextInput from "./TwoWayTextInput.vue";

describe("TwoWayTextInput", () => {
  it("shows an empty greeting initially", () => {
    const wrapper = mount(TwoWayTextInput);
    expect(wrapper.get("[data-testid='greeting']").text()).toBe("Hello, !");
  });

  it("updates the displayed name when the input value changes", async () => {
    const wrapper = mount(TwoWayTextInput);
    const input = wrapper.get("[data-testid='name-input']");

    await input.setValue("Ada");

    expect(wrapper.get("[data-testid='greeting']").text()).toBe("Hello, Ada!");
  });

  it("keeps updating as the input value changes again", async () => {
    const wrapper = mount(TwoWayTextInput);
    const input = wrapper.get("[data-testid='name-input']");

    await input.setValue("Grace");
    expect(wrapper.get("[data-testid='greeting']").text()).toBe("Hello, Grace!");

    await input.setValue("Ada Grace");
    expect(wrapper.get("[data-testid='greeting']").text()).toBe(
      "Hello, Ada Grace!",
    );
  });
});
