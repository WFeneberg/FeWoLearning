import { Injectable, computed, signal } from "@angular/core";

// Exercise 085 — a typed signal store: actions and selectors (advanced).
// Goal:   funnel every state change through one typed shape (an "action"), instead of a grab-bag
//         of imperative methods each free to mutate state however they like.
// Drills: a discriminated-union action type, a single pure reducer as the only place state can
//         change shape, `dispatch()` as the only mutation entry point, and computed selectors.
// Passes: when `npx jest exercises/03-advanced/ex085_signal_store_pattern` is green.
//
// Exercise 067 already keeps one writable signal behind read-only selectors and command methods.
// This store takes the next step: instead of one method per kind of change (`add`, `remove`,
// `clear`, each free to shape the next state however it likes), every change is described as data —
// a `CartAction` — and a single `reduce` function is the only place that turns "current state +
// action" into "next state." `dispatch()` never touches `state.lines` by hand; it only ever calls
// `reduce` and writes back whatever it returns.
//
// The payoff is the same reason Redux popularized this shape: `reduce` is a pure function with no
// dependency on the store, the signal, or Angular at all, so it can be tested directly with plain
// inputs and outputs — no TestBed, no injected service — while `dispatch()` on the store is tested
// separately to prove the wiring (signal update + selectors) is correct. Two independent surfaces,
// each simple to verify on its own.
//
// `reduce` must never mutate its `state` argument or the arrays inside it — always return a new
// `CartState` built from copies (exercise 030's discipline). Angular's change detection depends on
// this: signals notify subscribers by comparing the *reference* the setter received, so mutating the
// old object in place and handing that same reference back to `.update()` would look like no change
// happened at all.

export interface CartLine {
  readonly sku: string;
  readonly qty: number;
  readonly unitPrice: number;
}

export interface CartState {
  readonly lines: readonly CartLine[];
}

export const EMPTY_CART: CartState = { lines: [] };

export type CartAction =
  | { readonly type: "add"; readonly sku: string; readonly qty: number; readonly unitPrice: number }
  | { readonly type: "remove"; readonly sku: string }
  | { readonly type: "clear" };

/**
 * TODO: implement reduce — a pure function computing the next CartState from the current one and
 * an action. It must not mutate `state` or `state.lines`.
 *   - "add": if a line with the same sku already exists, replace it with one whose qty is the sum
 *     of both quantities and whose unitPrice is the new action's unitPrice (last write wins on
 *     price). Otherwise append a new line. A qty below 1 is a RangeError.
 *   - "remove": drop the line with that sku. An unknown sku is a no-op (same shape of state back,
 *     new reference is fine).
 *   - "clear": back to EMPTY_CART.
 */
export function reduce(state: CartState, action: CartAction): CartState {
  throw new Error("TODO: implement reduce");
}

@Injectable({ providedIn: "root" })
export class CartStore {
  private readonly state = signal<CartState>(EMPTY_CART);

  readonly lines = computed(() => this.state().lines);
  readonly itemCount = computed(() => this.lines().reduce((sum, line) => sum + line.qty, 0));
  readonly subtotal = computed(() =>
    this.lines().reduce((sum, line) => sum + line.qty * line.unitPrice, 0),
  );
  readonly isEmpty = computed(() => this.lines().length === 0);

  /** The only way CartState ever changes: hand an action to the reducer, write back its result. */
  dispatch(action: CartAction): void {
    this.state.update((current) => reduce(current, action));
  }
}
