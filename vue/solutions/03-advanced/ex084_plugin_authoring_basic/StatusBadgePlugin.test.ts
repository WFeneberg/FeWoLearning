import { describe, expect, it } from "vitest";
import { defineComponent } from "vue";
import { mount } from "@vue/test-utils";
import { statusBadgePlugin } from "./StatusBadgePlugin";

describe("statusBadgePlugin", () => {
  it("registers the badge globally under the default name", () => {
    const Host = defineComponent({
      template: `<StatusBadge status="ok" />`,
    });
    const wrapper = mount(Host, {
      global: { plugins: [statusBadgePlugin] },
    });
    expect(wrapper.find(".status-badge").text()).toBe("ok");
  });

  it("registers the badge under a custom name from install options", () => {
    const Host = defineComponent({
      template: `<Badge status="down" />`,
    });
    const wrapper = mount(Host, {
      global: {
        plugins: [[statusBadgePlugin, { componentName: "Badge" }]],
      },
    });
    expect(wrapper.find(".status-badge").text()).toBe("down");
  });

  it("exposes a global $statusPrefix configured at install time", () => {
    let captured = "";
    const Host = defineComponent({
      mounted() {
        captured = (this as unknown as { $statusPrefix: string }).$statusPrefix;
      },
      template: `<StatusBadge status="ok" />`,
    });
    mount(Host, {
      global: {
        plugins: [[statusBadgePlugin, { defaultPrefix: "SYS:" }]],
      },
    });
    expect(captured).toBe("SYS:");
  });

  it("defaults $statusPrefix to an empty string when no options are given", () => {
    let captured = "unset";
    const Host = defineComponent({
      mounted() {
        captured = (this as unknown as { $statusPrefix: string }).$statusPrefix;
      },
      template: `<StatusBadge status="ok" />`,
    });
    mount(Host, {
      global: { plugins: [statusBadgePlugin] },
    });
    expect(captured).toBe("");
  });

  it("keeps registrations isolated across separate app instances", () => {
    const HostA = defineComponent({ template: `<AlphaBadge status="a" />` });
    const HostB = defineComponent({ template: `<BetaBadge status="b" />` });

    const wrapperA = mount(HostA, {
      global: { plugins: [[statusBadgePlugin, { componentName: "AlphaBadge" }]] },
    });
    const wrapperB = mount(HostB, {
      global: { plugins: [[statusBadgePlugin, { componentName: "BetaBadge" }]] },
    });

    expect(wrapperA.find(".status-badge").text()).toBe("a");
    expect(wrapperB.find(".status-badge").text()).toBe("b");
  });
});
