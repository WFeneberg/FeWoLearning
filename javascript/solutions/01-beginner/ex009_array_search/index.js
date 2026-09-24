// Reference solution — exercise 009.

export function findUser(users, id) {
  return users.find((user) => user.id === id);
}

export function lastError(events) {
  return events.findLast((event) => event.level === "error");
}

export function hasAdmin(users) {
  return users.some((user) => user.role === "admin");
}

export function allActive(users) {
  return users.every((user) => user.active);
}

export function containsValue(list, value) {
  return list.includes(value);
}
