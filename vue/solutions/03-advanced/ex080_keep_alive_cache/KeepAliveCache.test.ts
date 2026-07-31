import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import KeepAliveCache from "./KeepAliveCache.vue";

describe("KeepAliveCache", () => {
  it("shows the Notes tab by default", () => {
    const wrapper = mount(KeepAliveCache);
    expect(wrapper.find('[data-testid="notes-input"]').exists()).toBe(true);
    expect(wrapper.find('[data-testid="info-panel"]').exists()).toBe(false);
  });

  it("switches to the Info tab and back", async () => {
    const wrapper = mount(KeepAliveCache);
    await wrapper.find('[data-testid="tab-btn-info"]').trigger("click");
    expect(wrapper.find('[data-testid="info-panel"]').exists()).toBe(true);
    expect(wrapper.find('[data-testid="notes-input"]').exists()).toBe(false);

    await wrapper.find('[data-testid="tab-btn-notes"]').trigger("click");
    expect(wrapper.find('[data-testid="notes-input"]').exists()).toBe(true);
  });

  it("retains typed text in the Notes tab after switching away and back", async () => {
    const wrapper = mount(KeepAliveCache);

    const notesInput = wrapper.find('[data-testid="notes-input"]');
    await notesInput.setValue("draft: remember the milk");
    expect((notesInput.element as HTMLTextAreaElement).value).toBe(
      "draft: remember the milk",
    );

    // Switch away to Info, then switch back to Notes.
    await wrapper.find('[data-testid="tab-btn-info"]').trigger("click");
    await wrapper.find('[data-testid="tab-btn-notes"]').trigger("click");

    const notesInputAgain = wrapper.find('[data-testid="notes-input"]');
    expect((notesInputAgain.element as HTMLTextAreaElement).value).toBe(
      "draft: remember the milk",
    );
  });
});
