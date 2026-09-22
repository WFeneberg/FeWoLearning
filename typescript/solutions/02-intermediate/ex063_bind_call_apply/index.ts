// Reference solution — exercise 063.
export interface Store {
  total: number;
}

export function addTo(this: Store, amount: number): number {
  this.total += amount;
  return this.total;
}

// The annotation follows addTo: add a parameter there and this keeps up.
export function detach(store: Store): OmitThisParameter<typeof addTo> {
  return addTo.bind(store);
}

export function preset(store: Store, amount: number): () => number {
  // bind fixes the receiver and the first argument in one go.
  return addTo.bind(store, amount);
}
