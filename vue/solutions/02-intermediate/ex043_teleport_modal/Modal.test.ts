import { afterEach, beforeEach, describe, expect, it } from "vitest";
import { mount } from "@vue/test-utils";
import Modal from "./Modal.vue";

describe("Modal", () => {
  let modalRoot: HTMLDivElement;

  beforeEach(() => {
    modalRoot = document.createElement("div");
    modalRoot.id = "modal-root";
    document.body.appendChild(modalRoot);
  });

  afterEach(() => {
    document.body.innerHTML = "";
  });

  it("teleports its content under #modal-root, not under its own subtree", () => {
    const wrapper = mount(Modal, {
      props: { open: true, title: "Booking confirmed" },
      attachTo: document.body,
    });

    // The component's own rendered subtree stays empty (or a comment
    // placeholder) — the modal markup does not live there.
    expect(wrapper.find(".modal-overlay").exists()).toBe(false);

    // The modal content is present in the DOM, under the teleport target.
    const teleported = modalRoot.querySelector(".modal-overlay");
    expect(teleported).not.toBeNull();
    expect(modalRoot.querySelector("h2")?.textContent).toBe(
      "Booking confirmed",
    );

    wrapper.unmount();
  });

  it("renders nothing when closed", () => {
    mount(Modal, {
      props: { open: false, title: "Hidden" },
      attachTo: document.body,
    });

    expect(modalRoot.querySelector(".modal-overlay")).toBeNull();
  });

  it("emits close when the close button is clicked", async () => {
    const wrapper = mount(Modal, {
      props: { open: true, title: "Booking confirmed" },
      attachTo: document.body,
    });

    const closeButton = modalRoot.querySelector<HTMLButtonElement>(
      "button.modal-close",
    );
    expect(closeButton).not.toBeNull();
    closeButton?.click();
    await wrapper.vm.$nextTick();

    expect(wrapper.emitted("close")).toHaveLength(1);

    wrapper.unmount();
  });
});
