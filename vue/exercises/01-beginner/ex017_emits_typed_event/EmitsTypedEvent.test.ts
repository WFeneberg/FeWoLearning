import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import EmitsTypedEvent from "./EmitsTypedEvent.vue";

describe("EmitsTypedEvent", () => {
  it("emits a 'submit' event on click", async () => {
    const wrapper = mount(EmitsTypedEvent);
    const button = wrapper.find("button");

    await button.trigger("click");

    expect(wrapper.emitted()).toHaveProperty("submit");
  });

  it("emits the 'submit' payload with the expected text", async () => {
    const wrapper = mount(EmitsTypedEvent);
    const button = wrapper.find("button");

    await button.trigger("click");

    expect(wrapper.emitted("submit")).toEqual([["hello"]]);
  });

  it("emits once per click", async () => {
    const wrapper = mount(EmitsTypedEvent);
    const button = wrapper.find("button");

    await button.trigger("click");
    await button.trigger("click");

    expect(wrapper.emitted("submit")).toHaveLength(2);
  });
});
