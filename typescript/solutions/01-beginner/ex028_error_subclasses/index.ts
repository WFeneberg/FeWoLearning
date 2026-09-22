// Reference solution — exercise 028.
export class ValidationError extends Error {
  readonly field: string;

  constructor(message: string, field: string, options?: { cause?: unknown }) {
    // The second argument is the standard ErrorOptions bag; passing
    // `options` straight through keeps `cause` even when it is absent.
    super(message, options);
    this.name = "ValidationError";
    this.field = field;
  }
}

export function causeChain(error: Error): string[] {
  const messages: string[] = [];
  let current: unknown = error;
  while (current instanceof Error) {
    messages.push(current.message);
    current = current.cause;
  }
  return messages;
}
