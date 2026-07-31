// Exercise 056 — shared injection key for the theme value.
import type { InjectionKey } from "vue";

export const themeKey: InjectionKey<string> = Symbol("theme");
