// Reference solution — exercise 037.

export async function loadProfile(api, id) {
  const user = await api.getUser(id);
  const orders = await api.getOrders(user.id);
  return { user, orders };
}

export async function safeAwait(promise) {
  try {
    return [null, await promise];
  } catch (error) {
    return [error, undefined];
  }
}

export async function failsLater(message) {
  throw new RangeError(message);
}

export async function awaitPlain(value, log) {
  log.push("before");
  const awaited = await value;
  log.push("after");
  return awaited;
}

export async function sumSequential(api, ids) {
  let total = 0;
  for (const id of ids) total += await api.getValue(id);
  return total;
}
