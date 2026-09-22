// Reference solution — exercise 008.
export type Callback = (line: string) => void;

export type UndefinedCallback = (line: string) => undefined;

export function forEachLine(text: string, onLine: Callback): void {
  for (const line of text.split("\n")) {
    onLine(line);
  }
}
