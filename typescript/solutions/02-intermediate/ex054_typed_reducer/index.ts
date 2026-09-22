// Reference solution — exercise 054.
export interface State {
  items: readonly string[];
  total: number;
}

export interface ActionMap {
  add: { item: string; amount: number };
  remove: { item: string };
  clear: Record<string, never>;
}

// The mapped type builds one tagged member per key; indexing it with
// keyof takes their union.
export type Action = {
  [K in keyof ActionMap]: { type: K } & ActionMap[K];
}[keyof ActionMap];

export function reduce(state: State, action: Action): State {
  switch (action.type) {
    case "add":
      return {
        items: [...state.items, action.item],
        total: state.total + action.amount,
      };
    case "remove":
      return {
        items: state.items.filter((item) => item !== action.item),
        total: state.total,
      };
    case "clear":
      return { items: [], total: 0 };
  }
}
