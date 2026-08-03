// Exercise 094 — Headless Disclosure component (reference solution).
import { defineComponent, ref } from "vue";

/** What the default slot receives. This is the component's entire API. */
export interface DisclosureSlotProps {
  isOpen: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
}

export const Disclosure = defineComponent({
  name: "Disclosure",
  props: {
    defaultOpen: { type: Boolean, default: false },
  },
  emits: {
    change: (open: boolean) => typeof open === "boolean",
  },
  setup(props, { slots, emit }) {
    const isOpen = ref(props.defaultOpen);

    // Funnel every mutation through one setter so "emit only on a real change"
    // holds for open(), close() and toggle() alike.
    const set = (next: boolean) => {
      if (isOpen.value === next) return;
      isOpen.value = next;
      emit("change", next);
    };

    const slotProps = (): DisclosureSlotProps => ({
      isOpen: isOpen.value,
      open: () => set(true),
      close: () => set(false),
      toggle: () => set(!isOpen.value),
    });

    // Returning the slot's children directly is what makes this renderless: no
    // element of our own is introduced. A slot call always yields an array, and
    // returning an array produces a Fragment — which has no single root node.
    // So unwrap the common single-child case, and only fall back to a fragment
    // when the consumer really did render several roots.
    return () => {
      const children = slots.default?.(slotProps());
      if (!children || children.length === 0) return null;
      return children.length === 1 ? children[0] : children;
    };
  },
});
