// @vitest-environment jsdom
import { mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import TemplateRefFocus from "./TemplateRefFocus.vue";

describe("TemplateRefFocus", () => {
  it("focuses the input element once mounted", () => {
    const wrapper = mount(TemplateRefFocus, { attachTo: document.body });

    const input = wrapper.get("input").element;
    expect(document.activeElement).toBe(input);

    wrapper.unmount();
  });
});
