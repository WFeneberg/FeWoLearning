// Ambient JSX typings for the JSX exercises (ex077).
//
// The .tsx files use the classic `/** @jsx h */` pragma rather than the automatic
// runtime, so TypeScript does not pull in Vue's own JSX types. This shim supplies
// them once, project-wide.
//
// It must live in exactly one file: when both the exercise and its solution
// declared `namespace JSX` themselves, `IntrinsicElements` ended up with two
// index signatures for `string` and TypeScript rejected it with TS2374.
import type { h } from "vue";

declare global {
  namespace JSX {
    interface Element extends ReturnType<typeof h> {}
    interface ElementClass {
      $props: unknown;
    }
    interface IntrinsicElements {
      [name: string]: unknown;
    }
  }
}

export {};
