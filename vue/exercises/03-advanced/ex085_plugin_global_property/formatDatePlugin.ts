// Exercise 085 — formatDatePlugin (advanced).
// Goal:   a Vue plugin that adds `$formatDate` to app.config.globalProperties,
//         formatting a Date (or date-time string) as "YYYY-MM-DD" (or with a
//         custom separator supplied via plugin options).
// Drills: Vue plugins (the Plugin/install contract), app.config.globalProperties,
//         augmenting ComponentCustomProperties, options-API access to
//         plugin-provided globals.
import type { App, Plugin } from "vue";

export type FormatDate = (date: Date | string) => string;

export interface FormatDatePluginOptions {
  /** Separator placed between the year, month and day segments. Default "-". */
  separator?: string;
}

declare module "vue" {
  interface ComponentCustomProperties {
    $formatDate: FormatDate;
  }
}

export const formatDatePlugin: Plugin<[FormatDatePluginOptions?]> = {
  install(_app: App, _options?: FormatDatePluginOptions) {
    throw new Error("TODO: implement formatDatePlugin");
  },
};
