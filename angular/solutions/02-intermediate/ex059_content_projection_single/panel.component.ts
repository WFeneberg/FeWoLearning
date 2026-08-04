import { Component, Input } from "@angular/core";

// Exercise 059 — single-slot content projection (reference solution).
@Component({
  selector: "app-panel",
  standalone: true,
  template: `
    <section class="panel">
      <h3 class="heading">{{ heading }}</h3>
      <div class="body">
        <!-- Fallback content, used only when the caller projects nothing. -->
        <ng-content>nothing here yet</ng-content>
      </div>
    </section>
  `,
})
export class PanelComponent {
  @Input() heading = "Panel";
}
