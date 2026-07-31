import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import TransitionFadeToggle from "./TransitionFadeToggle.vue";

// The default @vue/test-utils config auto-stubs <Transition>, which skips the
// real enter/leave class dance. Disabling that stub lets us assert the actual
// fade-enter-*/fade-leave-* classes Vue applies to the wrapped element.
const mountOptions = { global: { stubs: { transition: false } } };

describe("TransitionFadeToggle", () => {
  it("does not render the message initially", () => {
    const wrapper = mount(TransitionFadeToggle, mountOptions);
    expect(wrapper.find('[data-testid="message"]').exists()).toBe(false);
  });

  it("applies the fade enter classes when the message appears", async () => {
    const wrapper = mount(TransitionFadeToggle, mountOptions);

    await wrapper.find('[data-testid="toggle-btn"]').trigger("click");

    const message = wrapper.find('[data-testid="message"]');
    expect(message.exists()).toBe(true);
    expect(message.classes()).toContain("fade-enter-from");
    expect(message.classes()).toContain("fade-enter-active");
  });

  it("applies the fade leave classes when the message disappears", async () => {
    const wrapper = mount(TransitionFadeToggle, mountOptions);

    await wrapper.find('[data-testid="toggle-btn"]').trigger("click");
    await wrapper.find('[data-testid="toggle-btn"]').trigger("click");

    const message = wrapper.find('[data-testid="message"]');
    expect(message.exists()).toBe(true);
    expect(message.classes()).toContain("fade-leave-from");
    expect(message.classes()).toContain("fade-leave-active");
  });

  it("toggles visible back and forth via the exposed state", async () => {
    const wrapper = mount(TransitionFadeToggle, mountOptions);
    const vm = wrapper.vm as unknown as { visible: boolean };

    expect(vm.visible).toBe(false);
    await wrapper.find('[data-testid="toggle-btn"]').trigger("click");
    expect(vm.visible).toBe(true);
    await wrapper.find('[data-testid="toggle-btn"]').trigger("click");
    expect(vm.visible).toBe(false);
  });
});
