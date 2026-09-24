// Reference solution — exercise 014.

export function appendTo(value, list = []) {
  list.push(value);
  return list;
}

export function span(start, end = start + 10) {
  return { start, end };
}

export function greet(name = "guest") {
  return `Hello, ${name}!`;
}

export function withId(make, id = make()) {
  return id;
}
