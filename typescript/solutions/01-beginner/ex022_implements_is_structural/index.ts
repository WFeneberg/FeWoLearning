// Reference solution — exercise 022.
export interface Logger {
  readonly prefix: string;
  log(message: string): void;
}

export class SilentLogger {
  readonly prefix = "silent";
  readonly seen: string[] = [];

  log(message: string): void {
    this.seen.push(message);
  }
}

export class CollectingLogger implements Logger {
  readonly prefix = "collect";
  readonly lines: string[] = [];

  log(message: string): void {
    this.lines.push(`${this.prefix}: ${message}`);
  }
}

export type SatisfiesLogger<C> = C extends Logger ? true : false;

export function logAll(logger: Logger, messages: readonly string[]): void {
  for (const message of messages) {
    logger.log(message);
  }
}
