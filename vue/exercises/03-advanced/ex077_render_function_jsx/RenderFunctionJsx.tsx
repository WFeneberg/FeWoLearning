// Exercise 077 — RenderFunctionJsx component (advanced).
// Goal:   skip the SFC <template> entirely and use a JSX render function to
//         conditionally render one of two elements based on a `status` prop.
// Drills: defineComponent + setup() returning a JSX render function, the
//         classic `/** @jsx */` pragma (no template compiler involved),
//         conditional rendering driven by props instead of v-if in a template.
/** @jsx h */
/** @jsxFrag Fragment */
import { defineComponent, h, Fragment, type PropType } from "vue";

// No local ambient JSX declaration here: @vue/runtime-dom already ships global
// JSX typings, and re-declaring IntrinsicElements adds a second index signature
// for `string`, which TypeScript rejects (TS2374).

export type Status = "online" | "offline";

export default defineComponent({
  name: "RenderFunctionJsx",
  props: {
    status: {
      type: String as PropType<Status>,
      required: true,
    },
  },
  setup(_props) {
    throw new Error("TODO: implement RenderFunctionJsx render function");
  },
});
