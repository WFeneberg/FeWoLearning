import { describe, expect, it, vi } from "vitest";
import { flushPromises, mount } from "@vue/test-utils";
import { defineComponent, h } from "vue";
import { createRetryAsyncComponent } from "./AsyncComponentRetry";

const ResolvedComponent = defineComponent({
  name: "Resolved",
  render: () => h("div", { class: "resolved" }, "Loaded!"),
});

describe("createRetryAsyncComponent", () => {
  it("shows the loading state while the first load is pending", () => {
    const loader = vi.fn(() => new Promise(() => {}));
    const AsyncComp = createRetryAsyncComponent(loader);
    const wrapper = mount(AsyncComp);
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
    const AsyncComp = createRetryAsyncComponent(loader);
    const wrapper = mount(AsyncComp);

    await flushPromises();
    await flushPromises();

    expect(loader).toHaveBeenCalledTimes(2);
    expect(wrapper.find(".resolved").exists()).toBe(true);
    expect(wrapper.text()).toBe("Loaded!");
  });

  it("gives up after a second failure and renders the error component", async () => {
    const loader = vi.fn(() => Promise.reject(new Error("still failing")));
    const AsyncComp = createRetryAsyncComponent(loader);
    const wrapper = mount(AsyncComp);

    await flushPromises();
    await flushPromises();

    expect(loader).toHaveBeenCalledTimes(2);
    expect(wrapper.find(".async-error").exists()).toBe(true);
    expect(wrapper.text()).toContain("Failed to load component");
  });
});
