// Reference solution — exercise 008.

export function totalRevenue(orders) {
  return orders.reduce((sum, order) => sum + order.total, 0);
}

export function namesOfBigOrders(orders, min) {
  return orders.filter((order) => order.total >= min).map((order) => order.customer);
}

export function countByStatus(orders) {
  return orders.reduce((counts, order) => {
    counts[order.status] = (counts[order.status] ?? 0) + 1;
    return counts;
  }, {});
}

export function averageOrderValue(orders) {
  if (orders.length === 0) return null;
  return totalRevenue(orders) / orders.length;
}
