import { Component } from "@angular/core";

// Supporting infra for exercise 074 — a plain standalone component that stands in for "the
// expensive feature nobody should have to download until they actually visit /panel". It has
// nothing to do with the exercise itself; it just needs to live in its own module so that
// importing it dynamically is a real, separate `import()` call rather than a self-import.
@Component({
  selector: "app-lazy-panel",
  standalone: true,
  template: `<p class="lazy-panel">Lazy panel loaded</p>`,
})
export class LazyPanelComponent {}
