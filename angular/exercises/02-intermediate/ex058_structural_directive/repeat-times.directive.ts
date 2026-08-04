import { Component, Directive, Input, TemplateRef, ViewContainerRef, inject } from "@angular/core";

// Exercise 058 — a structural directive (intermediate).
// Goal:   stamp out (or withhold) a chunk of template from a directive.
// Drills: TemplateRef, ViewContainerRef, createEmbeddedView with a context object, clear(), the
//         `*` microsyntax and what it desugars to, and context variables via let-.
// Passes: when `npx jest exercises/02-intermediate/ex058_structural_directive` is green.
//
// The asterisk is pure sugar. `<p *appUnless="hidden">` becomes
// `<ng-template appUnless="hidden"><p></p></ng-template>`, and the directive is instantiated on
// that ng-template. That is why it can inject TemplateRef — the template it was put on — and
// ViewContainerRef, the place in the DOM where views can be inserted. @if and @for are the same
// machinery with compiler support.
//
// Two responsibilities, and it is the second that gets forgotten: create the view when it should
// exist, and *remove* it when it should not. A directive that only ever calls
// createEmbeddedView leaks a duplicate every time its input changes, which looks like the list
// growing on its own.
//
// The context object passed to createEmbeddedView is what `let-x` reads. `$implicit` is the one a
// bare `let-x` picks up (exercise 028), and named keys are read with `let-i="index"`.
//
// One easy thing to leave out: the setter has to be an actual @Input(). A plain setter named after
// the selector is never bound, so it simply never runs and the directive renders nothing at all —
// no error, no warning.
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="row" *appRepeatTimes="count; let i = index; let isLast = last">
//     {{ i }}{{ isLast ? "!" : "" }}
//   </p>
//   <p class="secret" *appUnless="hidden">visible</p>

export interface RepeatContext {
  readonly $implicit: number;
  readonly index: number;
  readonly count: number;
  readonly last: boolean;
}

/**
 * TODO: stamp the template `n` times.
 *
 * Each view gets a context of `{$implicit: index, index, count, last}`. Setting the input again
 * must rebuild from scratch — clear the container first, or views accumulate. A count below zero
 * is a RangeError; zero renders nothing.
 */
@Directive({
  selector: "[appRepeatTimes]",
  standalone: true,
})
export class RepeatTimesDirective {
  private readonly template = inject(TemplateRef<RepeatContext>);
  private readonly container = inject(ViewContainerRef);

  /** How many views the directive has created in total — the spec watches for leaks. */
  static viewsCreated = 0;

  @Input() set appRepeatTimes(count: number) {
    throw new Error("TODO: implement the appRepeatTimes setter");
  }
}

/**
 * TODO: the inverse of @if — render the template only while the condition is false.
 *
 * Must not create a second view when set to the same value twice, and must remove the view when
 * the condition becomes true.
 */
@Directive({
  selector: "[appUnless]",
  standalone: true,
})
export class UnlessDirective {
  private readonly template = inject(TemplateRef<unknown>);
  private readonly container = inject(ViewContainerRef);

  static viewsCreated = 0;

  @Input() set appUnless(condition: boolean) {
    throw new Error("TODO: implement the appUnless setter");
  }
}

@Component({
  selector: "app-repeat-host",
  standalone: true,
  // TODO: import both directives.
  template: `<p>TODO: render the host — see the template contract above</p>`,
})
export class RepeatHostComponent {
  count = 3;
  hidden = false;
}
