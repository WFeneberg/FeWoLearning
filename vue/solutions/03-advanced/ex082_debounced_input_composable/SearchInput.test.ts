import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { mount } from "@vue/test-utils";
import SearchInput from "./SearchInput.vue";

describe("SearchInput", () => {
  beforeEach(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("does not emit search immediately on input", async () => {
    const wrapper = mount(SearchInput, { props: { delay: 300 } });
    await wrapper.get('[data-testid="input"]').setValue("v");
    expect(wrapper.emitted("search")).toBeUndefined();
  });

  it("emits search once the delay elapses after typing stops", async () => {
    const wrapper = mount(SearchInput, { props: { delay: 300 } });
    await wrapper.get('[data-testid="input"]').setValue("vue");
    vi.advanceTimersByTime(299);
    expect(wrapper.emitted("search")).toBeUndefined();
    vi.advanceTimersByTime(1);
    expect(wrapper.emitted("search")).toEqual([["vue"]]);
  });

  it("collapses rapid keystrokes into a single emit of the final value", async () => {
    const wrapper = mount(SearchInput, { props: { delay: 300 } });
    const input = wrapper.get('[data-testid="input"]');

    await input.setValue("v");
    vi.advanceTimersByTime(100);
    await input.setValue("vu");
    vi.advanceTimersByTime(100);
    await input.setValue("vue");
    vi.advanceTimersByTime(100);
    await input.setValue("vue.js");
    // 300ms have elapsed in total, but each keystroke reset the timer, so
    // still nothing should have been emitted yet.
    expect(wrapper.emitted("search")).toBeUndefined();

    vi.advanceTimersByTime(300);
    const emissions = wrapper.emitted("search");
    expect(emissions).toHaveLength(1);
    expect(emissions?.[0]).toEqual(["vue.js"]);
  });

  it("uses a default delay of 300ms when none is provided", async () => {
    const wrapper = mount(SearchInput);
    await wrapper.get('[data-testid="input"]').setValue("hi");
    vi.advanceTimersByTime(299);
    expect(wrapper.emitted("search")).toBeUndefined();
    vi.advanceTimersByTime(1);
    expect(wrapper.emitted("search")).toEqual([["hi"]]);
  });

  it("restarts the wait when the delay prop differs", async () => {
    const wrapper = mount(SearchInput, { props: { delay: 500 } });
    await wrapper.get('[data-testid="input"]').setValue("a");
    vi.advanceTimersByTime(300);
    expect(wrapper.emitted("search")).toBeUndefined();
    vi.advanceTimersByTime(200);
    expect(wrapper.emitted("search")).toEqual([["a"]]);
  });
});
