import { Component } from "@angular/core";

// Exercise 060 — multi-slot content projection (reference solution).
@Component({
  selector: "app-card",
  standalone: true,
  template: `
    <article class="card">
      <header class="head"><ng-content select="[card-title]" /></header>
      <div class="media"><ng-content select="img" /></div>
      <div class="body"><ng-content select=".card-body" /></div>
      <footer class="foot"><ng-content select="[card-footer]" /></footer>
      <!-- Last on purpose: an unqualified slot claims whatever is left, so putting it first
           would swallow everything before the selective slots got a chance. -->
      <div class="rest"><ng-content /></div>
    </article>
  `,
})
export class CardComponent {}
