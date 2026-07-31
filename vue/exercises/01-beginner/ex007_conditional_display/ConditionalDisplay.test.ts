import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ConditionalDisplay from "./ConditionalDisplay.vue";

describe("ConditionalDisplay", () => {
  it("shows 'Loading' when status is loading", () => {
    const wrapper = mount(ConditionalDisplay, { props: { status: "loading" } });
    expect(wrapper.text()).toBe("Loading");
  });

  it("shows 'Error' when status is error", () => {
    const wrapper = mount(ConditionalDisplay, { props: { status: "error" } });
    expect(wrapper.text()).toBe("Error");
  });

  it("shows 'Ready' when status is ready", () => {
    const wrapper = mount(ConditionalDisplay, { props: { status: "ready" } });
    expect(wrapper.text()).toBe("Ready");
  });
});
