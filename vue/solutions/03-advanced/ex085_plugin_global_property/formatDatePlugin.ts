// Exercise 085 — formatDatePlugin (reference solution).
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

function pad(n: number): string {
  return n < 10 ? `0${n}` : `${n}`;
}

export const formatDatePlugin: Plugin<[FormatDatePluginOptions?]> = {
  install(app: App, options?: FormatDatePluginOptions) {
    const separator = options?.separator ?? "-";

    const formatDate: FormatDate = (date) => {
      const d = typeof date === "string" ? new Date(date) : date;
      const year = d.getFullYear();
      const month = pad(d.getMonth() + 1);
      const day = pad(d.getDate());
      return [year, month, day].join(separator);
    };

    app.config.globalProperties.$formatDate = formatDate;
  },
};
