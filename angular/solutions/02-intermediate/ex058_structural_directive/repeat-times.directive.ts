import { Component, Directive, Input, TemplateRef, ViewContainerRef, inject } from "@angular/core";

// Exercise 058 — a structural directive (reference solution).

export interface RepeatContext {
  readonly $implicit: number;
  readonly index: number;
  readonly count: number;
  readonly last: boolean;
}

@Directive({
  selector: "[appRepeatTimes]",
  standalone: true,
})
export class RepeatTimesDirective {
  // The ng-template the `*` desugaring created, and the slot to insert views into.
  private readonly template = inject(TemplateRef<RepeatContext>);
  private readonly container = inject(ViewContainerRef);

  static viewsCreated = 0;

  @Input() set appRepeatTimes(count: number) {
    if (count < 0) {
      throw new RangeError("count must not be negative");
    }
    // Without this the previous views stay, and every input change appends more.
    this.container.clear();
    for (let index = 0; index < count; index += 1) {
      this.container.createEmbeddedView(this.template, {
        // $implicit is what a bare `let-x` reads; the rest are read by name.
        $implicit: index,
        index,
        count,
        last: index === count - 1,
      });
      RepeatTimesDirective.viewsCreated += 1;
    }
  }
}

@Directive({
  selector: "[appUnless]",
  standalone: true,
})
export class UnlessDirective {
  private readonly template = inject(TemplateRef<unknown>);
  private readonly container = inject(ViewContainerRef);

  static viewsCreated = 0;

  /** Tracked so a repeated set of the same value does not rebuild the view. */
  private rendered = false;

  @Input() set appUnless(condition: boolean) {
    if (!condition && !this.rendered) {
      this.container.createEmbeddedView(this.template);
      UnlessDirective.viewsCreated += 1;
      this.rendered = true;
    } else if (condition && this.rendered) {
      this.container.clear();
      this.rendered = false;
    }
    // The setter runs on every change-detection pass, so both branches have to be guarded —
    // creating unconditionally would duplicate the content once per render.
  }
}

@Component({
  selector: "app-repeat-host",
  standalone: true,
  imports: [RepeatTimesDirective, UnlessDirective],
  template: `
    <p class="row" *appRepeatTimes="count; let i = index; let isLast = last">
      {{ i }}{{ isLast ? "!" : "" }}
    </p>
    <p class="secret" *appUnless="hidden">visible</p>
  `,
})
export class RepeatHostComponent {
  count = 3;
  hidden = false;
}
