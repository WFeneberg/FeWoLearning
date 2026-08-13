import { Component, ComponentRef, Type, ViewContainerRef, viewChild } from "@angular/core";

// Exercise 080 — dynamic component loading: ViewContainerRef.createComponent, inputs (reference solution).

@Component({
  selector: "app-dynamic-outlet",
  standalone: true,
  template: `<ng-container #anchor></ng-container>`,
})
export class DynamicOutletComponent {
  private readonly anchor = viewChild.required("anchor", { read: ViewContainerRef });

  private current: ComponentRef<unknown> | null = null;

  load<T>(component: Type<T>, inputs: Record<string, unknown> = {}): ComponentRef<T> {
    this.clear();

    const ref = this.anchor().createComponent(component);
    for (const [name, value] of Object.entries(inputs)) {
      ref.setInput(name, value);
    }

    this.current = ref;
    return ref;
  }

  clear(): void {
    this.current?.destroy();
    this.current = null;
  }
}
