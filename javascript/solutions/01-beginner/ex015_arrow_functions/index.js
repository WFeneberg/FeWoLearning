// Reference solution — exercise 015.

export const double = (n) => n * 2;

export const makeTagger = (tag) => (message) => `[${tag}] ${message}`;

// The parentheses are what stop `{` reading as a function body.
export const makeRecord = (id) => ({ id, ok: true });

export function makeCart(rate = 1) {
  return {
    rate,
    items: [],
    add(price) {
      this.items.push(price);
      return this;
    },
    total() {
      // The arrow borrows `this` from total(); a function expression here
      // would get undefined and throw.
      return this.items.reduce((sum, price) => sum + price * this.rate, 0);
    },
  };
}
