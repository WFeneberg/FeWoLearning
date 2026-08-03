// Exercise 100 — Accessibility-audited tabs widget (reference solution).
import { computed, defineComponent, h, nextTick, ref } from "vue";

export interface TabDefinition {
  id: string;
  label: string;
  content: string;
  disabled?: boolean;
}

export const Tabs = defineComponent({
  name: "Tabs",
  props: {
    tabs: { type: Array as () => TabDefinition[], required: true },
    label: { type: String, default: "Tabs" },
    defaultTabId: { type: String, default: undefined },
  },
  emits: {
    change: (id: string) => typeof id === "string",
  },
  setup(props, { emit }) {
    const enabled = computed(() => props.tabs.filter((t) => !t.disabled));

    const initial = (): string => {
      const wanted = props.tabs.find((t) => t.id === props.defaultTabId && !t.disabled);
      return wanted?.id ?? enabled.value[0]?.id ?? "";
    };

    const activeId = ref(initial());
    const activeTab = computed(() => props.tabs.find((t) => t.id === activeId.value));

    const tabEls = new Map<string, HTMLButtonElement>();

    const activate = (id: string, moveFocus: boolean): void => {
      const target = props.tabs.find((t) => t.id === id);
      if (!target || target.disabled || id === activeId.value) return;

      activeId.value = id;
      emit("change", id);

      if (moveFocus) {
        // Focus has to follow the selection, otherwise the arrow keys move the
        // visual state away from where the keyboard actually is.
        void nextTick(() => tabEls.get(id)?.focus());
      }
    };

    /** Steps `delta` positions through the *enabled* tabs only, wrapping. */
    const step = (delta: number): void => {
      const list = enabled.value;
      if (list.length === 0) return;
      const current = list.findIndex((t) => t.id === activeId.value);
      const next = (current + delta + list.length) % list.length;
      activate(list[next].id, true);
    };

    const onKeydown = (event: KeyboardEvent): void => {
      switch (event.key) {
        case "ArrowRight":
          step(1);
          break;
        case "ArrowLeft":
          step(-1);
          break;
        case "Home":
          if (enabled.value[0]) activate(enabled.value[0].id, true);
          break;
        case "End": {
          const last = enabled.value[enabled.value.length - 1];
          if (last) activate(last.id, true);
          break;
        }
        default:
          // Anything else belongs to the page, not to us.
          return;
      }
      event.preventDefault();
    };

    return () => {
      const tabNodes = props.tabs.map((tab) => {
        const selected = tab.id === activeId.value;
        return h(
          "button",
          {
            key: tab.id,
            type: "button",
            role: "tab",
            id: `tab-${tab.id}`,
            "aria-controls": `panel-${tab.id}`,
            "aria-selected": String(selected),
            // Roving tabindex: the tablist is one tab stop, arrows move within it.
            tabindex: selected ? "0" : "-1",
            ...(tab.disabled ? { "aria-disabled": "true" } : {}),
            ref: (el: unknown) => {
              if (el && el instanceof HTMLButtonElement) tabEls.set(tab.id, el);
              else tabEls.delete(tab.id);
            },
            onClick: () => activate(tab.id, false),
            onKeydown,
          },
          tab.label,
        );
      });

      const current = activeTab.value;

      return h("div", [
        h("div", { role: "tablist", "aria-label": props.label }, tabNodes),
        current
          ? h(
              "div",
              {
                role: "tabpanel",
                id: `panel-${current.id}`,
                "aria-labelledby": `tab-${current.id}`,
                tabindex: "0",
              },
              current.content,
            )
          : null,
      ]);
    };
  },
});
