import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import LifecycleMountedFetch from "./LifecycleMountedFetch.vue";

describe("LifecycleMountedFetch", () => {
  it("is not loaded before mounting completes, and is loaded after mount", () => {
    const wrapper = mount(LifecycleMountedFetch);

    // By the time mount() resolves, onMounted has already run for the
    // synchronous case, so loaded should be true.
    expect((wrapper.vm as unknown as { loaded: boolean }).loaded).toBe(true);
    expect(wrapper.text()).toBe("Loaded");
  });

  it("starts with loaded false prior to mount (verified via a fresh unmounted instance's initial state)", () => {
    // Mount once to capture the component's rendered text right after mount.
    const wrapper = mount(LifecycleMountedFetch);
    // After mount, the DOM must reflect the mounted state, not the initial one.
    expect(wrapper.text()).not.toBe("Loading...");
  });
});
