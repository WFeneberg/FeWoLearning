import { Component, DestroyRef, inject, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { ActivatedRoute } from "@angular/router";
import { EMPTY, switchMap, tap } from "rxjs";
import { ArticleLoader } from "./article-loader.service";

// Exercise 063 — route parameters as a stream (reference solution).
@Component({
  selector: "app-article",
  standalone: true,
  template: `
    <h2 class="title">{{ title() }}</h2>
    <p class="loads">{{ loadCount() }}</p>
  `,
})
export class ArticleComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly loader = inject(ArticleLoader);

  // The DestroyRef is captured here, where there *is* an injection context. Passing it to
  // takeUntilDestroyed() explicitly is what lets the operator be used from a plain method later —
  // and it keeps the operator generic, where a stored `takeUntilDestroyed()` would be pinned to
  // MonoTypeOperatorFunction<unknown> and widen every pipeline it appears in.
  private readonly destroyRef = inject(DestroyRef);

  readonly title = signal("");

  readonly loadCount = signal(0);

  readonly seenIds = signal<readonly string[]>([]);

  start(): void {
    this.route.paramMap
      .pipe(
        tap((params) => {
          const id = params.get("id") ?? "";
          this.seenIds.update((seen) => [...seen, id]);
        }),
        switchMap((params) => {
          const id = params.get("id");
          // EMPTY rather than []: it types as Observable<never>, so the pipeline's value type
          // stays string instead of widening to unknown.
          return id === null ? EMPTY : this.loader.load(id);
        }),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe((title) => {
        this.title.set(title);
        this.loadCount.update((n) => n + 1);
      });
  }

  startFromSnapshot(): void {
    // Deliberately wrong for a reused component: read once, never again.
    const id = this.route.snapshot.paramMap.get("id") ?? "";
    this.seenIds.update((seen) => [...seen, id]);
    if (id === "") {
      return;
    }
    this.loader
      .load(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((title) => {
        this.title.set(title);
        this.loadCount.update((n) => n + 1);
      });
  }
}
