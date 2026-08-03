// Exercise 077 — RenderFunctionJsx component (reference solution).
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
  setup(props) {
    return () =>
      props.status === "online" ? (
        <span class="status-online">Online</span>
      ) : (
        <span class="status-offline">Offline</span>
      );
  },
});
