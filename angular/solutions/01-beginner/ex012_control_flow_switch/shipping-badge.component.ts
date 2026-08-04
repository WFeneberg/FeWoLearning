import { Component, signal } from "@angular/core";

// Exercise 012 — ShippingBadgeComponent (reference solution).

export type ShippingState = "pending" | "transit" | "delivered" | "lost";
export type Carrier = "post" | "courier" | "pickup";

@Component({
  selector: "app-shipping-badge",
  standalone: true,
  template: `
    @switch (state()) {
      @case ("pending") {
        <p class="pending">Not shipped yet</p>
      }
      @case ("transit") {
        <p class="transit">On its way ({{ etaDays() }} days)</p>
      }
      @case ("delivered") {
        <p class="delivered">Delivered</p>
      }
      @default {
        <p class="unknown">Unknown state: {{ state() }}</p>
      }
    }

    <!-- No @default here: an unmatched value renders nothing, which is not an error. -->
    @switch (carrier()) {
      @case ("post") {
        <span class="post">Post</span>
      }
      @case ("courier") {
        <span class="courier">Courier</span>
      }
    }
  `,
})
export class ShippingBadgeComponent {
  readonly state = signal<ShippingState>("pending");
  readonly carrier = signal<Carrier>("post");

  etaDays(): number {
    const state = this.state();
    if (state === "delivered") {
      return 0;
    }
    if (state !== "transit") {
      // Better a loud failure than a plausible number the template would render.
      throw new RangeError(`no delivery estimate for state "${state}"`);
    }
    switch (this.carrier()) {
      case "post":
        return 2;
      case "courier":
        return 1;
      case "pickup":
        return 0;
    }
  }
}
