import { describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import ConfirmDialog from "./ConfirmDialog.vue";

describe("ConfirmDialog", () => {
  it("renders the message", () => {
    const wrapper = mount(ConfirmDialog, { props: { message: "Delete this item?" } });
    expect(wrapper.text()).toContain("Delete this item?");
  });

  it("emits confirm (with no payload) when the confirm button is clicked", async () => {
    const wrapper = mount(ConfirmDialog, { props: { message: "Are you sure?" } });
    const buttons = wrapper.findAll("button");
    await buttons[0].trigger("click");

    expect(wrapper.emitted()).toHaveProperty("confirm");
    expect(wrapper.emitted("confirm")).toHaveLength(1);
    expect(wrapper.emitted("confirm")?.[0]).toEqual([]);
    expect(wrapper.emitted("cancel")).toBeUndefined();
  });

  it("emits cancel (with no payload) when the cancel button is clicked", async () => {
    const wrapper = mount(ConfirmDialog, { props: { message: "Are you sure?" } });
    const buttons = wrapper.findAll("button");
    await buttons[1].trigger("click");

    expect(wrapper.emitted()).toHaveProperty("cancel");
    expect(wrapper.emitted("cancel")).toHaveLength(1);
    expect(wrapper.emitted("cancel")?.[0]).toEqual([]);
    expect(wrapper.emitted("confirm")).toBeUndefined();
  });

  it("records each click as a separate emission, in order, when clicked repeatedly", async () => {
    const wrapper = mount(ConfirmDialog, { props: { message: "Are you sure?" } });
    const buttons = wrapper.findAll("button");

    await buttons[0].trigger("click");
    await buttons[1].trigger("click");
    await buttons[0].trigger("click");

    expect(wrapper.emitted("confirm")).toHaveLength(2);
    expect(wrapper.emitted("cancel")).toHaveLength(1);
    expect(wrapper.emitted("confirm")).toEqual([[], []]);
    expect(wrapper.emitted("cancel")).toEqual([[]]);
  });
});
