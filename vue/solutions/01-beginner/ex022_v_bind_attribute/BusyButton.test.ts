import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import BusyButton from "./BusyButton.vue";

describe("BusyButton", () => {
  it("has no disabled attribute when isBusy is false", () => {
    const wrapper = mount(BusyButton, { props: { isBusy: false } });
    expect(wrapper.find("button").attributes("disabled")).toBeUndefined();
  });

  it("has the disabled attribute when isBusy is true", () => {
    const wrapper = mount(BusyButton, { props: { isBusy: true } });
    expect(wrapper.find("button").attributes("disabled")).toBeDefined();
  });

  it("updates the disabled attribute reactively when the prop changes", async () => {
    const wrapper = mount(BusyButton, { props: { isBusy: false } });
    expect(wrapper.find("button").attributes("disabled")).toBeUndefined();

    await wrapper.setProps({ isBusy: true });
    expect(wrapper.find("button").attributes("disabled")).toBeDefined();
  });
});
