import { Component, ComponentRef, Type, ViewContainerRef, viewChild } from "@angular/core";

// Exercise 080 — dynamic component loading: ViewContainerRef.createComponent, inputs (advanced).
// Goal:   mount a component whose type is only known at runtime — a plugin panel, a chosen widget
//         from a list, anything a `@switch` over a fixed set of tags cannot express.
// Drills: ViewContainerRef.createComponent(), ComponentRef.setInput(), and ComponentRef.destroy()
//         (via the ref's own hostView) to tear down whatever was mounted before.
// Passes: when `npx jest exercises/03-advanced/ex080_dynamic_component_loading` is green.
//
// `@if` / `@for` / `@switch` all still require the template author to have written every branch
// in advance — the compiler needs to know, at build time, every component type that could ever
// appear. A `ViewContainerRef` breaks that requirement: `createComponent(SomeType)` accepts any
// `Type<T>` handed to it at runtime, which is exactly what a plugin system or a "renders whatever
// the server said the layout should be" screen needs.
//
// `ViewContainerRef` is not something you create — every element (and every `ng-container`) has
// one available as its "the spot in the DOM where I can insert views" handle, retrievable via DI at
// that node. `<ng-container #anchor></ng-container>` on its own renders nothing (a `ng-container`
// is never itself a DOM node); reading its `ViewContainerRef` is just asking "what can I insert
// right here?"
//
// `ComponentRef.setInput(name, value)` exists because a dynamically created component was never
// written into a template, so there is no `[name]="value"` binding for Angular to evaluate — inputs
// have to be pushed onto the instance by hand, one at a time, through the ref. And because nothing
// automatically destroys a dynamically created component when it is replaced (there is no template
// binding driving an `@if` to tear it down), the outlet has to track its own previous ComponentRef
// and destroy it itself before creating the next one — otherwise every call to `load()` would leak
// the component before it, left running with no way back to it.

@Component({
  selector: "app-dynamic-outlet",
  standalone: true,
  template: `<ng-container #anchor></ng-container>`,
})
export class DynamicOutletComponent {
  private readonly anchor = viewChild.required("anchor", { read: ViewContainerRef });

  private current: ComponentRef<unknown> | null = null;

  /**
   * TODO: implement load — destroy whatever this outlet currently hosts (see clear()), then create
   * a fresh instance of `component` in this outlet's container, push every entry of `inputs` onto
   * it via setInput(), and return the new ComponentRef.
   *
   * `inputs` is a plain string-keyed record rather than `Partial<T>` on purpose: `T`'s signal
   * inputs are typed as `InputSignal<string>`, not `string`, so a caller could never actually
   * satisfy `Partial<T>` with the plain values setInput() expects.
   */
  load<T>(component: Type<T>, inputs: Record<string, unknown> = {}): ComponentRef<T> {
    throw new Error("TODO: implement load");
  }

  /** TODO: implement clear — destroy the currently hosted component, if there is one. */
  clear(): void {
    throw new Error("TODO: implement clear");
  }
}
