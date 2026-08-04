import { Component, signal } from "@angular/core";

// Exercise 012 — ShippingBadgeComponent (beginner).
// Goal:   pick one of several branches by value, with @switch.
// Drills: @switch / @case / @default, the fact that cases do not fall through, and
//         choosing @switch over a chain of @else if.
// Passes: when `npx jest exercises/01-beginner/ex012_control_flow_switch` is green.
//
// @switch compares with === against the expression, and there is no `break` because
// there is no fall-through — one case renders and the rest are not in the DOM. Unlike
// JavaScript's switch it is not a statement you can leak out of, so a value with no
// matching @case renders @default, and a template with no @default renders nothing.
//
// Reach for @switch when you are testing one expression against several constants, and
// for @if when the branches test unrelated conditions. The spec covers a state with no
// @case on purpose, because "nothing rendered" is a real outcome worth knowing about.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   @switch (state()) {
//     @case ("pending") {
//       <p class="pending">Not shipped yet</p>
//     }
//     @case ("transit") {
//       <p class="transit">On its way ({{ etaDays() }} days)</p>
//     }
//     @case ("delivered") {
//       <p class="delivered">Delivered</p>
//     }
//     @default {
//       <p class="unknown">Unknown state: {{ state() }}</p>
//     }
//   }
//
//   <!-- A second switch, deliberately without a @default. -->
//   @switch (carrier()) {
//     @case ("post") {
//       <span class="post">Post</span>
//     }
//     @case ("courier") {
//       <span class="courier">Courier</span>
//     }
//   }

export type ShippingState = "pending" | "transit" | "delivered" | "lost";
export type Carrier = "post" | "courier" | "pickup";

@Component({
  selector: "app-shipping-badge",
  standalone: true,
  template: `<p>TODO: render the badge — see the template contract above</p>`,
})
export class ShippingBadgeComponent {
  readonly state = signal<ShippingState>("pending");
  readonly carrier = signal<Carrier>("post");

  /**
   * Days until arrival.
   *
   * - "delivered" is 0.
   * - "transit" depends on the carrier: 2 for the post, 1 for a courier, 0 for pickup.
   * - "pending" and "lost" have no estimate at all — throw a RangeError rather than
   *   inventing a number the template would happily render.
   */
  etaDays(): number {
    throw new Error("TODO: implement etaDays");
  }
}
