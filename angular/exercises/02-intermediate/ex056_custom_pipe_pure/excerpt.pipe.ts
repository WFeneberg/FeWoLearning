import { Component, Pipe, PipeTransform, signal } from "@angular/core";

// Exercise 056 — a pure custom pipe (intermediate).
// Goal:   write a PipeTransform, and understand what "pure" buys you.
// Drills: @Pipe + PipeTransform, extra arguments, purity meaning "re-run only when an input
//         changes by reference", and why that makes a pipe cheap to use in a big list.
// Passes: when `npx jest exercises/02-intermediate/ex056_custom_pipe_pure` is green.
//
// A pipe is pure by default, and pure has a precise meaning: Angular calls transform() again only
// when the input value or one of the arguments changes *by reference*. On every other
// change-detection pass it reuses the previous result. In a list of 500 rows that is the difference
// between 500 calls per render and 500 calls once.
//
// The rule cuts both ways, and this is the bit that surprises people. `{{ items | myPipe }}` where
// `items` is mutated in place — pushed to, sorted — does not re-run, because the reference did not
// change. It is the same reference-equality rule as signals (exercises 030 and 031), enforced in a
// different place, and the same fix applies: produce a new array rather than editing the old one.
//
// Which is also why a pure pipe must actually *be* pure. Angular caches its output, so a transform
// that reads the clock or a mutable service returns a stale answer and there is nothing to debug.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="default">{{ text() | excerpt }}</p>
//   <p class="short">{{ text() | excerpt: 5 }}</p>
//   <p class="custom">{{ text() | excerpt: 5 : "..." }}</p>
//   <p class="joined">{{ words() | excerpt: 8 }}</p>

/**
 * TODO: a standalone pipe named "excerpt".
 *
 * transform(value, length = 10, suffix = "…"):
 *   - a string no longer than `length` is returned unchanged
 *   - a longer one is cut to `length` characters with `suffix` appended
 *   - an array is joined with ", " first, then treated as a string
 *   - a `length` below 1 is a RangeError
 *
 * Count every real call in the static `calls` counter, so the spec can observe the caching.
 */
@Pipe({
  name: "excerpt",
  standalone: true,
})
export class ExcerptPipe implements PipeTransform {
  /** Incremented on each actual invocation. Reset by the spec between tests. */
  static calls = 0;

  transform(value: string | readonly string[], length = 10, suffix = "…"): string {
    throw new Error("TODO: implement transform");
  }
}

// A host so the spec can observe when Angular does and does not call the pipe.
@Component({
  selector: "app-excerpt-host",
  standalone: true,
  // TODO: import ExcerptPipe.
  template: `<p>TODO: render the excerpts — see the template contract above</p>`,
})
export class ExcerptHostComponent {
  readonly text = signal("hello");
  readonly words = signal<readonly string[]>(["one", "two"]);

  /** Mutate the array in place, without replacing it — deliberately the wrong way. */
  pushWordInPlace(word: string): void {
    throw new Error("TODO: implement pushWordInPlace");
  }

  /** Replace the array with a new one — the right way. */
  addWord(word: string): void {
    throw new Error("TODO: implement addWord");
  }
}
