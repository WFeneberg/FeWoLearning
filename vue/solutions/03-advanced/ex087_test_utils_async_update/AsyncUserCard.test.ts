import { afterEach, describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import AsyncUserCard from "./AsyncUserCard.vue";

describe("AsyncUserCard", () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it("shows the idle placeholder before loading", () => {
    const wrapper = mount(AsyncUserCard);
    expect(wrapper.text()).toContain("No user loaded yet");
  });

  it("does not show the fetched user until the awaited promise resolves and the DOM is flushed", async () => {
    vi.useFakeTimers();
    const wrapper = mount(AsyncUserCard);

    await wrapper.find("button").trigger("click");

    // The click handler ran synchronously up to its first `await`, so the
    // "loading" status is visible, but the simulated network call has not
    // resolved yet (the fake timer has not been advanced) — the user's name
    // must NOT appear in the DOM at this point.
    expect(wrapper.text()).toContain("Loading...");
    expect(wrapper.text()).not.toContain("Ada Lovelace");

    // Advance the fake clock past the simulated network delay and let
    // pending microtasks (the promise chain inside `load`) settle.
    await vi.advanceTimersByTimeAsync(500);
    // Vue batches reactive DOM updates into the next microtask tick — the
    // component's ref was just updated above, so the render is only
    // guaranteed to have flushed after an explicit `$nextTick()`.
    await wrapper.vm.$nextTick();

    expect(wrapper.text()).toContain("Ada Lovelace");
    expect(wrapper.text()).not.toContain("Loading...");
  });
});
