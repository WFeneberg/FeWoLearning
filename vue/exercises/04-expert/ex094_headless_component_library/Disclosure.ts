// Exercise 094 — Headless Disclosure component (expert).
// Goal:   a *renderless* component: it owns the open/closed behaviour and hands
//         it to the consumer through a scoped slot, but renders no markup of its
//         own. Whatever the slot returns is what lands in the DOM — no wrapper
//         element, no classes, no opinions about styling.
// Drills: renderless components, scoped slots as the public API, slot props,
//         returning slot children directly from a render function, emit on
//         genuine state changes only.
import { defineComponent } from "vue";

/** What the default slot receives. This is the component's entire API. */
export interface DisclosureSlotProps {
  isOpen: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
}

/**
 * A headless disclosure. Usage:
 *
 * ```vue
 * <Disclosure v-slot="{ isOpen, toggle }">
 *   <button @click="toggle">toggle</button>
 *   <p v-if="isOpen">content</p>
 * </Disclosure>
 * ```
 *
 * Requirements:
 *  - render **only** the default slot's content — never a wrapper element;
 *  - a slot call always returns an *array*. Returning that array makes a
 *    Fragment, which has no single root node, so unwrap the single-child case
 *    and return the lone VNode; fall back to the array for several roots;
 *  - if there is no default slot (or it renders nothing), render nothing;
 *  - emit `change` with the new boolean whenever the state actually flips, and
 *    not when a call is a no-op (`open()` on an already-open disclosure).
 */
export const Disclosure = defineComponent({
  name: "Disclosure",
  props: {
    /** Initial state. Defaults to closed. */
    defaultOpen: { type: Boolean, default: false },
  },
  emits: {
    change: (open: boolean) => typeof open === "boolean",
  },
  setup(_props, _ctx) {
    throw new Error("TODO: implement the Disclosure render function");
  },
});
