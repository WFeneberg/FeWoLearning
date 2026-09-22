// Reference solution — exercise 055.
export type Kind = "draft" | "published" | "archived";

export type CompleteMap<K extends string, V> = Record<K, V>;

export const LABELS: CompleteMap<Kind, string> = {
  draft: "Draft",
  published: "Published",
  archived: "Archived",
};

export function labelOf(kind: Kind): string {
  return LABELS[kind];
}

export function assertNever(value: never): never {
  throw new Error(`Unexpected value: ${JSON.stringify(value)}`);
}

export function statusOf(kind: Kind): string {
  switch (kind) {
    case "draft":
      return "not yet";
    case "published":
      return "live";
    case "archived":
      return "gone";
    default:
      // Reachable only once Kind gains a member — and then this line, not
      // the caller, is where the compiler complains.
      return assertNever(kind);
  }
}
