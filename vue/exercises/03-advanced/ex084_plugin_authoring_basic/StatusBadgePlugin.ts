// Exercise 084 — StatusBadgePlugin (advanced).
// Goal:   author a Vue plugin object whose `install(app, options)` hook
//         registers a global component (so any part of the app can use it
//         by tag name, with zero local imports) and exposes an
//         install-time-configurable global property.
// Drills: the `Plugin` interface, `app.component()`, `app.config
//         .globalProperties`, per-app install isolation, default options.
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
  install(_app: App, _options: StatusBadgeOptions = {}) {
    throw new Error("TODO: implement statusBadgePlugin.install");
  },
};
