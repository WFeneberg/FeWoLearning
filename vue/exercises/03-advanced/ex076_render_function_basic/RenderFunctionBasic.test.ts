import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RenderFunctionBasic from "./RenderFunctionBasic";

describe("RenderFunctionBasic", () => {
  it("renders an h1 with the title text by default", () => {
    const wrapper = mount(RenderFunctionBasic, { props: { title: "Hello World" } });
    const heading = wrapper.find("h1");
    expect(heading.exists()).toBe(true);
    expect(heading.text()).toBe("Hello World");
  });

  it("derives a kebab-case id from the title", () => {
    const wrapper = mount(RenderFunctionBasic, { props: { title: "Hello World" } });
    expect(wrapper.find("h1").attributes("id")).toBe("hello-world");
  });

  it("renders the requested heading level as the tag name", () => {
    const wrapper = mount(RenderFunctionBasic, { props: { title: "Section", level: 3 } });
    expect(wrapper.find("h3").exists()).toBe(true);
    expect(wrapper.find("h1").exists()).toBe(false);
    expect(wrapper.find("h3").text()).toBe("Section");
  });

  it("clamps out-of-range levels to the nearest valid heading", () => {
    const wrapper = mount(RenderFunctionBasic, { props: { title: "Too Big", level: 9 } });
    expect(wrapper.find("h6").exists()).toBe(true);
  });

  it("appends default slot content in an extra span, after the title", () => {
    const wrapper = mount(RenderFunctionBasic, {
      props: { title: "With Extra" },
      slots: { default: "note" },
    });
    const heading = wrapper.find("h1");
    expect(heading.find("span.render-heading-extra").text()).toBe("note");
    expect(heading.text()).toBe("With Extranote");
  });

  it("does not render an extra span when no slot content is provided", () => {
    const wrapper = mount(RenderFunctionBasic, { props: { title: "Plain" } });
    expect(wrapper.find("span.render-heading-extra").exists()).toBe(false);
  });
});
