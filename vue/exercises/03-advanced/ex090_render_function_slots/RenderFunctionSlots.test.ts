import { describe, expect, it } from "vitest";
import { defineComponent } from "vue";
import { mount } from "@vue/test-utils";
import RenderFunctionSlots from "./RenderFunctionSlots";

describe("RenderFunctionSlots", () => {
  it("renders default slot content from a parent template inside the container", () => {
    const Parent = defineComponent({
      components: { RenderFunctionSlots },
      template: `
        <RenderFunctionSlots>
          <p class="note">Hello from parent</p>
        </RenderFunctionSlots>
      `,
    });

    const wrapper = mount(Parent);
    const container = wrapper.find(".render-function-slots");

    expect(container.exists()).toBe(true);
    expect(container.find("p.note").exists()).toBe(true);
    expect(container.find("p.note").text()).toBe("Hello from parent");
  });

  it("renders multiple slot children in document order", () => {
    const Parent = defineComponent({
      components: { RenderFunctionSlots },
      template: `
        <RenderFunctionSlots>
          <span class="first">one</span>
          <span class="second">two</span>
        </RenderFunctionSlots>
      `,
    });

    const wrapper = mount(Parent);
    const spans = wrapper.findAll(".render-function-slots span").map((el) => el.text());
    expect(spans).toEqual(["one", "two"]);
  });

  it("renders an empty container when no slot content is provided", () => {
    const wrapper = mount(RenderFunctionSlots);
    const container = wrapper.find(".render-function-slots");
    expect(container.exists()).toBe(true);
    expect(container.text()).toBe("");
  });

  it("reacts to parent state changes affecting slot content", async () => {
    const Parent = defineComponent({
      components: { RenderFunctionSlots },
      data() {
        return { count: 1 };
      },
      template: `
        <RenderFunctionSlots>
          <span class="count">{{ count }}</span>
        </RenderFunctionSlots>
      `,
    });

    const wrapper = mount(Parent);
    expect(wrapper.find(".count").text()).toBe("1");

    await wrapper.setData({ count: 2 });
    expect(wrapper.find(".count").text()).toBe("2");
  });
});
