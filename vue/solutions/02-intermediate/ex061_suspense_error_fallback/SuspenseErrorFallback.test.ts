import { flushPromises, mount } from "@vue/test-utils";
import { describe, expect, it } from "vitest";
import SuspenseErrorFallback from "./SuspenseErrorFallback.vue";

describe("SuspenseErrorFallback", () => {
  it("shows the fallback loading state before the async setup settles", () => {
    const wrapper = mount(SuspenseErrorFallback, {
      props: { shouldFail: false },
    });
    expect(wrapper.find(".loading").exists()).toBe(true);
    expect(wrapper.find(".error").exists()).toBe(false);
  });

  it("renders the async content once the promise resolves", async () => {
    const wrapper = mount(SuspenseErrorFallback, {
      props: { shouldFail: false },
    });
    await flushPromises();
    expect(wrapper.find(".content").text()).toBe("Loaded!");
    expect(wrapper.find(".error").exists()).toBe(false);
  });

  it("renders the error message once the async setup's promise rejects", async () => {
    const wrapper = mount(SuspenseErrorFallback, {
      props: { shouldFail: true },
    });
    await flushPromises();
    expect(wrapper.find(".error").exists()).toBe(true);
    expect(wrapper.find(".error").text()).toBe("Failed to load data");
    expect(wrapper.find(".loading").exists()).toBe(false);
    expect(wrapper.find(".content").exists()).toBe(false);
  });
});
