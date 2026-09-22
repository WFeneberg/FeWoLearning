// Reference solution — exercise 020.
export interface Order {
  id: string;
  customer: { name: string; email: string };
  lines: { sku: string; qty: number }[];
}

export type CustomerName = Order["customer"]["name"];

export type OrderLine = Order["lines"][number];

export type OrderValue = Order[keyof Order];

export function totalQty(order: Order): number {
  return order.lines.reduce((sum, line) => sum + line.qty, 0);
}

export function lineAt(order: Order, index: number): OrderLine | undefined {
  return order.lines[index];
}
