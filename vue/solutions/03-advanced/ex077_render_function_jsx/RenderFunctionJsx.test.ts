import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import RenderFunctionJsx from "./RenderFunctionJsx";

describe("RenderFunctionJsx", () => {
  it("renders the online branch when status is online", () => {
    const wrapper = mount(RenderFunctionJsx, { props: { status: "online" } });
    expect(wrapper.find(".status-online").exists()).toBe(true);
    expect(wrapper.find(".status-offline").exists()).toBe(false);
    expect(wrapper.text()).toBe("Online");
  });

  it("renders the offline branch when status is offline", () => {
    const wrapper = mount(RenderFunctionJsx, { props: { status: "offline" } });
    expect(wrapper.find(".status-offline").exists()).toBe(true);
    expect(wrapper.find(".status-online").exists()).toBe(false);
    expect(wrapper.text()).toBe("Offline");
  });
});
