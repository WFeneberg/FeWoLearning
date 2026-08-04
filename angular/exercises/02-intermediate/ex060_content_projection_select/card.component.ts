import { Component } from "@angular/core";

// Exercise 060 — multi-slot content projection (intermediate).
// Goal:   route different pieces of the caller's content into different places.
// Drills: <ng-content select="…"> with an attribute, a class and an element selector, the
//         catch-all slot, and the fact that unmatched content is discarded entirely.
// Passes: when `npx jest exercises/02-intermediate/ex060_content_projection_select` is green.
//
// `select` takes a CSS selector and claims the matching top-level projected nodes. Slots are tried
// in template order and each node goes to the *first* slot that matches it, so an unqualified
// <ng-content> must come last or it swallows everything.
//
// The behaviour to know: content matching no slot is not rendered at all. There is no warning and
// nothing left over — it silently disappears, which is a genuinely hard bug to spot when a card
// renders with one section missing.
//
// Only top-level nodes are matched. A node nested inside a projected element travels with its
// parent, so select="[card-footer]" will not reach into a <div> to find one.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <article class="card">
//     <header class="head"><ng-content select="[card-title]" /></header>
//     <div class="media"><ng-content select="img" /></div>
//     <div class="body"><ng-content select=".card-body" /></div>
//     <footer class="foot"><ng-content select="[card-footer]" /></footer>
//     <div class="rest"><ng-content /></div>
//   </article>

@Component({
  selector: "app-card",
  standalone: true,
  template: `<p>TODO: render the card — see the template contract above</p>`,
})
export class CardComponent {}
