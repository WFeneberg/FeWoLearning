// Exercise 076 — RenderFunctionBasic component (advanced).
// Goal:   define a component with no <template>, using only a `render()`
//         function built from `h()`, that outputs a heading element for a
//         `title` prop at a configurable `level` (h1..h6), with an id slug
//         derived from the title and an optional default-slot extra node.
// Drills: options-API `render()`, `h()`, dynamic tag names, `this.$slots`.
import { defineComponent, h } from "vue";

export default defineComponent({
  name: "RenderFunctionBasic",
  props: {
    title: { type: String, required: true },
    level: { type: Number, default: 1 },
  },
  render() {
    throw new Error("TODO: implement render() using h()");
  },
});
