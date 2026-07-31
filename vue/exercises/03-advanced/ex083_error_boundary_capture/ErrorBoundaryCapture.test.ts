import { describe, expect, it } from "vitest";
import { defineComponent, h, nextTick } from "vue";
import { mount } from "@vue/test-utils";
import ErrorBoundaryCapture from "./ErrorBoundaryCapture.vue";

const ThrowingChild = defineComponent({
  name: "ThrowingChild",
  render() {
    throw new Error("Boom: child failed to render");
  },
});

const OkChild = defineComponent({
  name: "OkChild",
  render() {
    return h("p", "All good");
  },
});

describe("ErrorBoundaryCapture", () => {
  it("renders the default slot when no error is thrown", () => {
    const wrapper = mount(ErrorBoundaryCapture, {
      slots: { default: () => h(OkChild) },
    });
    expect(wrapper.text()).toContain("All good");
  });

  it("catches a child's thrown render error and shows the fallback instead", async () => {
    const wrapper = mount(ErrorBoundaryCapture, {
      slots: { default: () => h(ThrowingChild) },
    });
    await nextTick();
    expect(wrapper.text()).toContain("Something went wrong.");
    expect(wrapper.text()).not.toContain("All good");
  });

  it("uses a custom fallback message when provided as a prop", async () => {
    const wrapper = mount(ErrorBoundaryCapture, {
      props: { fallbackMessage: "Oops, that broke." },
      slots: { default: () => h(ThrowingChild) },
    });
    await nextTick();
    expect(wrapper.text()).toContain("Oops, that broke.");
  });
});
