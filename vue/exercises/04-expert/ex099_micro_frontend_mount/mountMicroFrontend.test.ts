import { afterEach, describe, expect, it, vi } from "vitest";
import { defineComponent, h, inject, nextTick, ref } from "vue";
import { activeMountCount, mountMicroFrontend } from "./mountMicroFrontend";

const labelKey = Symbol("label");

/** Owns local state, so two instances must not influence each other. */
const Counter = defineComponent({
  name: "Counter",
  props: { start: { type: Number, default: 0 } },
  setup(props) {
    const count = ref(props.start);
    return () =>
      h("button", { class: "counter", onClick: () => (count.value += 1) }, String(count.value));
  },
});

const LabelReader = defineComponent({
  name: "LabelReader",
  setup() {
    const label = inject(labelKey, "(none)");
    return () => h("span", { class: "label" }, String(label));
  },
});

const containers: HTMLElement[] = [];
function makeContainer(): HTMLElement {
  const el = document.createElement("div");
  document.body.appendChild(el);
  containers.push(el);
  return el;
}

afterEach(() => {
  while (containers.length) containers.pop()!.remove();
});

describe("mountMicroFrontend", () => {
  it("renders the component into the given container", () => {
    const container = makeContainer();

    mountMicroFrontend(Counter, container, { props: { start: 5 } });

    expect(container.querySelector(".counter")?.textContent).toBe("5");
  });

  it("rejects a container that is not an Element", () => {
    expect(() => mountMicroFrontend(Counter, null as never)).toThrow(TypeError);
  });

  it("keeps two instances of the same component fully independent", async () => {
    const a = makeContainer();
    const b = makeContainer();

    mountMicroFrontend(Counter, a, { props: { start: 0 } });
    mountMicroFrontend(Counter, b, { props: { start: 100 } });

    (a.querySelector(".counter") as HTMLButtonElement).click();
    await nextTick();

    expect(a.querySelector(".counter")?.textContent).toBe("1");
    expect(b.querySelector(".counter")?.textContent).toBe("100");
  });

  it("resolves app-level provides per instance without leaking", () => {
    const a = makeContainer();
    const b = makeContainer();

    mountMicroFrontend(LabelReader, a, { provides: { [labelKey]: "app-a" } });
    mountMicroFrontend(LabelReader, b, { provides: { [labelKey]: "app-b" } });
    const c = makeContainer();
    mountMicroFrontend(LabelReader, c);

    expect(a.querySelector(".label")?.textContent).toBe("app-a");
    expect(b.querySelector(".label")?.textContent).toBe("app-b");
    // No provide at all: the component's own inject default applies.
    expect(c.querySelector(".label")?.textContent).toBe("(none)");
  });

  it("calls configure with the app before mounting", () => {
    const container = makeContainer();
    const configure = vi.fn((app) => {
      // Nothing is rendered yet at this point.
      expect(container.innerHTML).toBe("");
      app.provide(labelKey, "from-configure");
    });

    mountMicroFrontend(LabelReader, container, { configure });

    expect(configure).toHaveBeenCalledTimes(1);
    expect(container.querySelector(".label")?.textContent).toBe("from-configure");
  });

  it("unmount empties the container and flips isMounted", () => {
    const container = makeContainer();
    const handle = mountMicroFrontend(Counter, container);

    expect(handle.isMounted).toBe(true);

    handle.unmount();

    expect(handle.isMounted).toBe(false);
    expect(container.innerHTML).toBe("");
  });

  it("unmount is idempotent", () => {
    const container = makeContainer();
    const handle = mountMicroFrontend(Counter, container);

    handle.unmount();
    expect(() => handle.unmount()).not.toThrow();
    expect(container.innerHTML).toBe("");
  });

  it("tracks how many instances are live", () => {
    const before = activeMountCount();
    const a = makeContainer();
    const b = makeContainer();

    const h1 = mountMicroFrontend(Counter, a);
    const h2 = mountMicroFrontend(Counter, b);
    expect(activeMountCount()).toBe(before + 2);

    h1.unmount();
    expect(activeMountCount()).toBe(before + 1);

    // A repeat unmount must not double-decrement.
    h1.unmount();
    expect(activeMountCount()).toBe(before + 1);

    h2.unmount();
    expect(activeMountCount()).toBe(before);
  });

  it("exposes the container and the app instance on the handle", () => {
    const container = makeContainer();
    const handle = mountMicroFrontend(Counter, container);

    expect(handle.container).toBe(container);
    expect(typeof handle.app.unmount).toBe("function");

    handle.unmount();
  });
});
