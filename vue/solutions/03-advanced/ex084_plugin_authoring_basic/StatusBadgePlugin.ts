// Exercise 084 — StatusBadgePlugin (advanced, reference solution).
import { defineComponent, type App, type Plugin } from "vue";

export interface StatusBadgeOptions {
  /** Global component name the badge is registered under. */
  componentName?: string;
  /** Prefix exposed as `this.$statusPrefix` inside every component. */
  defaultPrefix?: string;
}

/** The component the plugin registers globally — never imported directly by consumers. */
export const StatusBadge = defineComponent({
  name: "StatusBadge",
  props: {
    status: { type: String, required: true },
  },
  template: `<span class="status-badge">{{ status }}</span>`,
});

export const statusBadgePlugin: Plugin<[StatusBadgeOptions?]> = {
  install(app: App, options: StatusBadgeOptions = {}) {
    const { componentName = "StatusBadge", defaultPrefix = "" } = options;
    app.component(componentName, StatusBadge);
    app.config.globalProperties.$statusPrefix = defaultPrefix;
  },
};
