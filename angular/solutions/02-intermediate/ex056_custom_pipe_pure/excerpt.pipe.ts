import { Component, Pipe, PipeTransform, signal } from "@angular/core";

// Exercise 056 — a pure custom pipe (reference solution).

@Pipe({
  name: "excerpt",
  standalone: true,
  // Pure by default. Angular caches the result and re-runs only when an input changes by
  // reference, which is what makes a pipe cheap inside a long list.
})
export class ExcerptPipe implements PipeTransform {
  static calls = 0;

  transform(value: string | readonly string[], length = 10, suffix = "…"): string {
    ExcerptPipe.calls += 1;
    if (length < 1) {
      throw new RangeError("length must be at least 1");
    }
    const text = typeof value === "string" ? value : value.join(", ");
    return text.length <= length ? text : `${text.slice(0, length)}${suffix}`;
  }
}

@Component({
  selector: "app-excerpt-host",
  standalone: true,
  imports: [ExcerptPipe],
  template: `
    <p class="default">{{ text() | excerpt }}</p>
    <p class="short">{{ text() | excerpt: 5 }}</p>
    <p class="custom">{{ text() | excerpt: 5 : "..." }}</p>
    <p class="joined">{{ words() | excerpt: 8 }}</p>
  `,
})
export class ExcerptHostComponent {
  readonly text = signal("hello");
  readonly words = signal<readonly string[]>(["one", "two"]);

  pushWordInPlace(word: string): void {
    // Deliberately wrong on two counts at once: it mutates the signal's array in place, so
    // neither the signal nor the pure pipe sees a change.
    (this.words() as string[]).push(word);
  }

  addWord(word: string): void {
    this.words.update((words) => [...words, word]);
  }
}
