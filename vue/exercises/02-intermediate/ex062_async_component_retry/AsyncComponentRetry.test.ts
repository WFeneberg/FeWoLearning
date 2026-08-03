import { describe, expect, it, vi } from "vitest";
import { flushPromises, mount } from "@vue/test-utils";
import { defineComponent, h, type Component } from "vue";
import { createRetryAsyncComponent } from "./AsyncComponentRetry";

const ResolvedComponent = defineComponent({
  name: "Resolved",
  render: () => h("div", { class: "resolved" }, "Loaded!"),
});

/**
 * An async component has to be rendered as a *child* to observe its
 * loading/error/resolved states: mounting one as the root component leaves the
 * wrapper pinned to the first render, so it would appear stuck on "Loading..."
 * forever no matter how often you flush.
 */
function mountAsChild(AsyncComp: Component) {
  return mount(defineComponent({ render: () => h(AsyncComp) }));
}

describe("createRetryAsyncComponent", () => {
  it("shows the loading state while the first load is pending", () => {
    const loader = vi.fn(() => new Promise<Component>(() => {}));
    const wrapper = mountAsChild(createRetryAsyncComponent(loader));

    expect(wrapper.text()).toContain("Loading...");
  });

  it("retries once after a failed load and renders the resolved component", async () => {
    let attempts = 0;
    const loader = vi.fn(() => {
      attempts += 1;
      if (attempts === 1) {
        return Promise.reject(new Error("network fail"));
      }
      return Promise.resolve(ResolvedComponent);
    });
    const wrapper = mountAsChild(createRetryAsyncComponent(loader));

    await flushPromises();

    expect(loader).toHaveBeenCalledTimes(2);
    expect(wrapper.find(".resolved").exists()).toBe(true);
    expect(wrapper.text()).toBe("Loaded!");
  });

  it("gives up after a second failure and renders the error component", async () => {
    const loader = vi.fn(() => Promise.reject(new Error("still failing")));
    const wrapper = mountAsChild(createRetryAsyncComponent(loader));

    await flushPromises();

    // Exactly one retry: the loader runs twice, then `fail()` ends it.
    expect(loader).toHaveBeenCalledTimes(2);
    expect(wrapper.find(".async-error").exists()).toBe(true);
    expect(wrapper.text()).toContain("Failed to load component");
  });
});
