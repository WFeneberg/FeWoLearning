import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ToggleVisibility from "./ToggleVisibility.vue";

describe("ToggleVisibility", () => {
  it("shows the message initially", () => {
    const wrapper = mount(ToggleVisibility);
    expect(wrapper.find('[data-testid="message"]').exists()).toBe(true);
  });

  it("removes the message after one click", async () => {
    const wrapper = mount(ToggleVisibility);
    await wrapper.find("button").trigger("click");
    expect(wrapper.find('[data-testid="message"]').exists()).toBe(false);
  });

  it("shows the message again after a second click", async () => {
    const wrapper = mount(ToggleVisibility);
    await wrapper.find("button").trigger("click");
    await wrapper.find("button").trigger("click");
    expect(wrapper.find('[data-testid="message"]').exists()).toBe(true);
  });
});
