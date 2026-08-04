import { AsyncPipe } from "@angular/common";
import { Component } from "@angular/core";
import { BehaviorSubject, Observable, Subject } from "rxjs";

// Exercise 055 — AsyncPipe (reference solution).
@Component({
  selector: "app-feed",
  standalone: true,
  imports: [AsyncPipe],
  template: `
    <p class="count">{{ count$ | async }}</p>

    <!-- "as" both waits for a value and gives the block a non-null binding. -->
    @if (title$ | async; as title) {
      <h2 class="title">{{ title }}</h2>
    } @else {
      <p class="waiting">waiting</p>
    }

    <!-- Two pipes on one source: two subscriptions. Over a cold HTTP observable, two requests. -->
    <p class="twice-a">{{ tracked$ | async }}</p>
    <p class="twice-b">{{ tracked$ | async }}</p>

    <!-- One pipe, one subscription, value read twice. This is the fix. -->
    @if (shared$ | async; as value) {
      <p class="shared-a">{{ value }}</p>
      <p class="shared-b">{{ value }}</p>
    }
  `,
})
export class FeedComponent {
  readonly count$ = new BehaviorSubject(0);

  readonly title$ = new Subject<string>();

  trackedSubscriptions = 0;

  sharedSubscriptions = 0;

  // The subscribe function runs once per subscriber, which is what makes the count meaningful.
  readonly tracked$ = new Observable<string>((subscriber) => {
    this.trackedSubscriptions += 1;
    subscriber.next("tracked");
  });

  readonly shared$ = new Observable<string>((subscriber) => {
    this.sharedSubscriptions += 1;
    subscriber.next("shared");
  });

  setCount(value: number): void {
    this.count$.next(value);
  }

  setTitle(value: string): void {
    this.title$.next(value);
  }
}
