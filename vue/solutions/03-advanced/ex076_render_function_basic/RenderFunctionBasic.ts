// Exercise 076 — RenderFunctionBasic component (advanced, reference solution).
import { defineComponent, h, type VNodeChild } from "vue";

export default defineComponent({
  name: "RenderFunctionBasic",
  props: {
    title: { type: String, required: true },
    level: { type: Number, default: 1 },
  },
  render() {
    const level = Math.min(6, Math.max(1, this.level));
    const tag = `h${level}`;
    const id = this.title
      .trim()
      .toLowerCase()
      .replace(/\s+/g, "-");

    const children: VNodeChild[] = [this.title];
    const extra = this.$slots.default?.();
    if (extra) {
      children.push(h("span", { class: "render-heading-extra" }, extra));
    }

    return h(tag, { class: "render-heading", id }, children);
  },
});
