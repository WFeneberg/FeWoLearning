import { describe, expect, it, afterEach } from "vitest";
import { mount } from "@vue/test-utils";
import TeleportTargetChange, { TELEPORT_TARGET } from "./TeleportTargetChange.vue";

function makeTeleportTarget() {
  const el = document.createElement("div");
  el.id = TELEPORT_TARGET.slice(1);
  document.body.appendChild(el);
  return el;
}

describe("TeleportTargetChange", () => {
  afterEach(() => {
    document.body.innerHTML = "";
  });

  it("renders content in place while the overlay is closed", () => {
    const target = makeTeleportTarget();
    const wrapper = mount(TeleportTargetChange, { attachTo: document.body });

    const content = wrapper.get('[data-testid="content"]').element;
    expect(target.contains(content)).toBe(false);
    expect(wrapper.element.contains(content)).toBe(true);

    wrapper.unmount();
  });

  it("teleports content to the target once opened", async () => {
    const target = makeTeleportTarget();
    const wrapper = mount(TeleportTargetChange, { attachTo: document.body });

    await wrapper.get('[data-testid="toggle"]').trigger("click");

    const content = document.querySelector('[data-testid="content"]') as HTMLElement;
    expect(target.contains(content)).toBe(true);
    expect(wrapper.element.contains(content)).toBe(false);

    wrapper.unmount();
  });

  it("moves content back in place when toggled closed again", async () => {
    const target = makeTeleportTarget();
    const wrapper = mount(TeleportTargetChange, { attachTo: document.body });

    const toggle = () => wrapper.get('[data-testid="toggle"]').trigger("click");
    await toggle();
    await toggle();

    const content = wrapper.get('[data-testid="content"]').element;
    expect(target.contains(content)).toBe(false);
    expect(wrapper.element.contains(content)).toBe(true);

    wrapper.unmount();
  });
});
