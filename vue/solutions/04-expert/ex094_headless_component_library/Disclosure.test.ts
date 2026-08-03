import { describe, expect, it } from "vitest";
import { h } from "vue";
import { mount } from "@vue/test-utils";
import { Disclosure, type DisclosureSlotProps } from "./Disclosure";

/** Mounts the disclosure with a slot that renders a button plus optional content. */
function mountDisclosure(defaultOpen = false) {
  return mount(Disclosure, {
    props: { defaultOpen },
    slots: {
      default: (slotProps: DisclosureSlotProps) => [
        h("button", { onClick: slotProps.toggle }, "toggle"),
        slotProps.isOpen ? h("p", { class: "panel" }, "content") : null,
      ],
    },
  });
}

describe("Disclosure", () => {
  it("renders the slot content with no wrapper element of its own", () => {
    const wrapper = mount(Disclosure, {
      slots: { default: () => h("p", { class: "only-child" }, "hi") },
    });

    // The rendered root is the slot's own element, not a div the component added.
    expect(wrapper.element.tagName).toBe("P");
    expect(wrapper.find(".only-child").exists()).toBe(true);
  });

  it("starts closed by default and exposes isOpen to the slot", () => {
    const wrapper = mountDisclosure();

    expect(wrapper.find(".panel").exists()).toBe(false);
  });

  it("honours defaultOpen", () => {
    const wrapper = mountDisclosure(true);

    expect(wrapper.find(".panel").exists()).toBe(true);
  });

  it("toggles the state through the slot prop", async () => {
    const wrapper = mountDisclosure();

    await wrapper.get("button").trigger("click");
    expect(wrapper.find(".panel").exists()).toBe(true);

    await wrapper.get("button").trigger("click");
    expect(wrapper.find(".panel").exists()).toBe(false);
  });

  it("open() and close() are idempotent and emit only on a real change", async () => {
    let api!: DisclosureSlotProps;
    const wrapper = mount(Disclosure, {
      slots: {
        default: (slotProps: DisclosureSlotProps) => {
          api = slotProps;
          return h("span", String(slotProps.isOpen));
        },
      },
    });

    api.open();
    await wrapper.vm.$nextTick();
    expect(wrapper.text()).toBe("true");
    expect(wrapper.emitted("change")).toEqual([[true]]);

    // Already open: no state change, so no further emit.
    api.open();
    await wrapper.vm.$nextTick();
    expect(wrapper.emitted("change")).toEqual([[true]]);

    api.close();
    await wrapper.vm.$nextTick();
    expect(wrapper.text()).toBe("false");
    expect(wrapper.emitted("change")).toEqual([[true], [false]]);

    api.close();
    await wrapper.vm.$nextTick();
    expect(wrapper.emitted("change")).toEqual([[true], [false]]);
  });

  it("renders several slot roots without wrapping them", () => {
    const wrapper = mount(Disclosure, {
      slots: {
        default: () => [h("span", { class: "a" }, "a"), h("span", { class: "b" }, "b")],
      },
    });

    expect(wrapper.find(".a").exists()).toBe(true);
    expect(wrapper.find(".b").exists()).toBe(true);
  });

  it("renders nothing when no default slot is supplied", () => {
    const wrapper = mount(Disclosure);

    expect(wrapper.html()).toBe("");
  });
});
