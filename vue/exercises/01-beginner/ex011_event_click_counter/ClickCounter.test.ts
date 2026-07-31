import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ClickCounter from "./ClickCounter.vue";

describe("ClickCounter", () => {
  it("starts at 0", () => {
    const wrapper = mount(ClickCounter);
    expect(wrapper.text()).toContain("Count: 0");
  });

  it("increments count by 1 on each click", async () => {
    const wrapper = mount(ClickCounter);
    const button = wrapper.find("button");

    await button.trigger("click");
    expect(wrapper.text()).toContain("Count: 1");

    await button.trigger("click");
    await button.trigger("click");
    expect(wrapper.text()).toContain("Count: 3");
  });
});
