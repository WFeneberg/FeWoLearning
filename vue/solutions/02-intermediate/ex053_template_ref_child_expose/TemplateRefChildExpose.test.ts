import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import TemplateRefChildExpose from "./TemplateRefChildExpose.vue";

describe("TemplateRefChildExpose (template ref to child component with expose)", () => {
  it("starts with the child counter at zero", () => {
    const wrapper = mount(TemplateRefChildExpose);
    expect(wrapper.find(".child-count").text()).toBe("Count: 0");
  });

  it("increments the child's internal state independently of the parent", async () => {
    const wrapper = mount(TemplateRefChildExpose);

    await wrapper.find(".child-increment").trigger("click");
    await wrapper.find(".child-increment").trigger("click");
    await wrapper.find(".child-increment").trigger("click");

    expect(wrapper.find(".child-count").text()).toBe("Count: 3");
  });

  it("resets the child's state through the parent's exposed reset() call", async () => {
    const wrapper = mount(TemplateRefChildExpose);

    await wrapper.find(".child-increment").trigger("click");
    await wrapper.find(".child-increment").trigger("click");
    expect(wrapper.find(".child-count").text()).toBe("Count: 2");

    await wrapper.find(".reset-child").trigger("click");

    expect(wrapper.find(".child-count").text()).toBe("Count: 0");
  });

  it("allows incrementing again after a reset", async () => {
    const wrapper = mount(TemplateRefChildExpose);

    await wrapper.find(".child-increment").trigger("click");
    await wrapper.find(".reset-child").trigger("click");
    await wrapper.find(".child-increment").trigger("click");
    await wrapper.find(".child-increment").trigger("click");

    expect(wrapper.find(".child-count").text()).toBe("Count: 2");
  });
});
