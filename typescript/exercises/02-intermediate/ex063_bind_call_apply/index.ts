// Exercise 063 — bind, call, apply, and removing `this` from a signature
// (intermediate).
// Goal:   attach a receiver once and hand out a plain function afterwards.
// Drills: strictBindCallApply, OmitThisParameter, partial application.
// Passes: addTo demands a Store, detach produces something that no longer
//         does, and the partially applied form keeps the remaining
//         parameters.
//
// Under `strict` — which turns on strictBindCallApply — `call`, `apply`
// and `bind` are typed properly rather than accepting anything: the
// receiver is checked against the function's `this` parameter (ex062) and
// the arguments against its real ones. `fn.call({}, 1)` on a function
// declaring `this: Store` is a compile error, and `fn.bind(store)` gives
// back a function whose `this` requirement is GONE.
//
// `OmitThisParameter<T>` is the type of that result, and it is the return
// type `detach` should carry. Writing `(amount: number) => number` by hand
// would work today and drift the moment addTo gains a parameter.
//
// `bind` also does partial application: `fn.bind(store, 5)` fixes both the
// receiver and the first argument, and the type reflects it. That is the
// second half of the row.

export interface Store {
  total: number;
}

/** TODO: add `amount` to the receiver's total and return it. Declare what
 *  this must be called on. */
export function addTo(_amount: number): number {
  throw new Error("TODO: implement addTo");
}

/** TODO: bind `store` in, giving back a function that needs no receiver.
 *  Type the result so it follows addTo rather than repeating it. */
export function detach(_store: Store): OmitThisParameter<typeof addTo> {
  throw new Error("TODO: implement detach");
}

/** TODO: bind both the receiver AND the amount, giving back a function of
 *  no arguments. Use bind for both, not a wrapper lambda. */
export function preset(_store: Store, _amount: number): () => number {
  throw new Error("TODO: implement preset");
}
