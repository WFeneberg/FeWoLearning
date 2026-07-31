// Exercise 077 — RenderFunctionJsx component (reference solution).
/** @jsx h */
/** @jsxFrag Fragment */
import { defineComponent, h, Fragment, type PropType } from "vue";

declare global {
  namespace JSX {
    interface Element extends ReturnType<typeof h> {}
    interface IntrinsicElements {
      [name: string]: any;
    }
  }
}

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
