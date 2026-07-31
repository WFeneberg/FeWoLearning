import { describe, expect, it } from "vitest";
import { mount, flushPromises } from "@vue/test-utils";
import { defineComponent, h, Suspense } from "vue";
import SuspenseAsyncComponent from "./SuspenseAsyncComponent.vue";

// A minimal wrapper that puts the async component under a <Suspense>
// boundary with a distinct fallback, mirroring how a real app would use it.
const Wrapper = defineComponent({
  name: "Wrapper",
  setup() {
    return () =>
      h(Suspense, null, {
        default: () => h(SuspenseAsyncComponent),
        fallback: () => h("div", "Loading profile..."),
      });
  },
});

describe("SuspenseAsyncComponent", () => {
  it("renders the fallback slot before the async setup resolves", () => {
    const wrapper = mount(Wrapper);

    expect(wrapper.text()).toBe("Loading profile...");
  });

  it("renders the resolved profile after the async setup completes", async () => {
    const wrapper = mount(Wrapper);
    expect(wrapper.text()).toBe("Loading profile...");

    await flushPromises();

    expect(wrapper.text()).not.toBe("Loading profile...");
    expect(wrapper.get('[data-testid="name"]').text()).toBe("Ada Lovelace");
    expect(wrapper.get('[data-testid="role"]').text()).toBe("Engineer");
  });
});
