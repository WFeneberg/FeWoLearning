import { describe, expect, it } from "vitest";
import { defineComponent, h, nextTick } from "vue";
import { mount } from "@vue/test-utils";
import { useHistory } from "./useHistory";

describe("useHistory", () => {
  it("starts at the initial value with nothing to undo or redo", () => {
    const h1 = useHistory("a");

    expect(h1.current.value).toBe("a");
    expect(h1.canUndo.value).toBe(false);
    expect(h1.canRedo.value).toBe(false);
    expect(h1.undoCount.value).toBe(0);
  });

  it("set advances the value and enables undo", () => {
    const h1 = useHistory("a");

    h1.set("b");

    expect(h1.current.value).toBe("b");
    expect(h1.canUndo.value).toBe(true);
    expect(h1.canRedo.value).toBe(false);
    expect(h1.undoCount.value).toBe(1);
  });

  it("undo and redo walk the history", () => {
    const h1 = useHistory("a");
    h1.set("b");
    h1.set("c");

    h1.undo();
    expect(h1.current.value).toBe("b");
    expect(h1.canRedo.value).toBe(true);

    h1.undo();
    expect(h1.current.value).toBe("a");
    expect(h1.canUndo.value).toBe(false);

    h1.redo();
    expect(h1.current.value).toBe("b");

    h1.redo();
    expect(h1.current.value).toBe("c");
    expect(h1.canRedo.value).toBe(false);
  });

  it("undo and redo are no-ops at the ends", () => {
    const h1 = useHistory("a");

    h1.undo();
    expect(h1.current.value).toBe("a");

    h1.set("b");
    h1.redo();
    expect(h1.current.value).toBe("b");
  });

  it("a new set after an undo clears the redo stack", () => {
    const h1 = useHistory("a");
    h1.set("b");
    h1.set("c");
    h1.undo(); // back at "b", "c" is redoable

    h1.set("d");

    expect(h1.current.value).toBe("d");
    expect(h1.canRedo.value).toBe(false);

    h1.undo();
    expect(h1.current.value).toBe("b");
  });

  it("setting the current value again does not grow the history", () => {
    const h1 = useHistory("a");

    h1.set("a");
    h1.set("a");

    expect(h1.canUndo.value).toBe(false);
    expect(h1.undoCount.value).toBe(0);
  });

  it("drops the oldest entries once capacity is exceeded", () => {
    const h1 = useHistory(0, 2);

    h1.set(1);
    h1.set(2);
    h1.set(3);

    // Only the two most recent edits remain undoable.
    expect(h1.undoCount.value).toBe(2);

    h1.undo();
    expect(h1.current.value).toBe(2);
    h1.undo();
    expect(h1.current.value).toBe(1);

    // 0 fell off the front.
    expect(h1.canUndo.value).toBe(false);
    expect(h1.current.value).toBe(1);
  });

  it("a capacity of 0 keeps no history at all", () => {
    const h1 = useHistory("a", 0);

    h1.set("b");

    expect(h1.current.value).toBe("b");
    expect(h1.canUndo.value).toBe(false);
    expect(h1.undoCount.value).toBe(0);
  });

  it("rejects a negative capacity", () => {
    expect(() => useHistory("a", -1)).toThrow(RangeError);
  });

  it("clear drops the history but keeps the value", () => {
    const h1 = useHistory("a");
    h1.set("b");
    h1.set("c");
    h1.undo();

    h1.clear();

    expect(h1.current.value).toBe("b");
    expect(h1.canUndo.value).toBe(false);
    expect(h1.canRedo.value).toBe(false);
  });

  it("drives a component re-render", async () => {
    const Comp = defineComponent({
      setup() {
        const hist = useHistory("first");
        return { hist };
      },
      render() {
        return h("span", this.hist.current.value);
      },
    });

    const wrapper = mount(Comp);
    expect(wrapper.text()).toBe("first");

    wrapper.vm.hist.set("second");
    await nextTick();
    expect(wrapper.text()).toBe("second");

    wrapper.vm.hist.undo();
    await nextTick();
    expect(wrapper.text()).toBe("first");
  });
});
