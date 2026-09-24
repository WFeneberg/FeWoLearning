// Reference solution — exercise 025.

export class Stack {
  constructor() {
    this.items = [];
  }

  push(value) {
    this.items.push(value);
    return this;
  }

  pop() {
    return this.items.pop();
  }

  peek() {
    return this.items.at(-1);
  }

  get size() {
    return this.items.length;
  }

  static from(iterable) {
    const stack = new Stack();
    for (const value of iterable) stack.push(value);
    return stack;
  }
}
