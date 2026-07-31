// Exercise 090 — RenderFunctionSlots component (advanced, reference solution).
import { defineComponent, h } from "vue";

export default defineComponent({
  name: "RenderFunctionSlots",
  setup(_props, { slots }) {
    return () =>
      h(
        "div",
        { class: "render-function-slots" },
        slots.default?.(),
      );
  },
});
