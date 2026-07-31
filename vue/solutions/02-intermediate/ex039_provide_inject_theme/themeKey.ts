// Exercise 039 — shared injection key for the theme provide/inject pair.
import type { InjectionKey } from "vue";

export const themeKey = Symbol("theme") as InjectionKey<string>;
