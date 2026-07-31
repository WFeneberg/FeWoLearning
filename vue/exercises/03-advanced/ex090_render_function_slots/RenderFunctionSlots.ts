// Exercise 090 — RenderFunctionSlots component (advanced).
// Goal:   define a component with no <template>, using only a `setup()` +
//         render function built from `h()`, that wraps whatever the parent
//         passes as default slot children inside a styled container element.
// Drills: `setup(props, { slots })`, `h()`, invoking `slots.default?.()`,
//         forwarding arbitrary slot content instead of hard-coded children.
import { defineComponent, h } from "vue";

export default defineComponent({
  name: "RenderFunctionSlots",
  setup(_props, { slots }) {
    return () => {
      throw new Error("TODO: implement render using slots.default?.()");
    };
  },
});
