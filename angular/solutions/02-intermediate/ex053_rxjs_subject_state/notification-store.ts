import { Injectable } from "@angular/core";
import { BehaviorSubject, Observable, ReplaySubject, Subject } from "rxjs";

// Exercise 053 — Subjects as state (reference solution).

export interface Notice {
  readonly id: number;
  readonly text: string;
}

@Injectable({ providedIn: "root" })
export class NotificationStore {
  // BehaviorSubject for state: it always has a current value, so a late subscriber sees it.
  private readonly unreadCount = new BehaviorSubject(0);

  // Plain Subject for events: no memory, which is right for "something just happened".
  private readonly arrivals = new Subject<Notice>();

  // ReplaySubject for a short history.
  private readonly recent = new ReplaySubject<Notice>(3);

  // asObservable() seals the write end — same stream, no next().
  readonly unreadCount$: Observable<number> = this.unreadCount.asObservable();

  readonly arrivals$: Observable<Notice> = this.arrivals.asObservable();

  readonly recent$: Observable<Notice> = this.recent.asObservable();

  currentCount(): number {
    // Only a BehaviorSubject offers this: "the current value" is a meaningful question for it.
    return this.unreadCount.value;
  }

  notify(notice: Notice): void {
    this.arrivals.next(notice);
    this.recent.next(notice);
    this.unreadCount.next(this.unreadCount.value + 1);
  }

  markAllRead(): void {
    this.unreadCount.next(0);
  }

  isSealed(): boolean {
    const hasNext = (stream: Observable<unknown>): boolean =>
      typeof (stream as unknown as { next?: unknown }).next === "function";
    return !hasNext(this.unreadCount$) && !hasNext(this.arrivals$) && !hasNext(this.recent$);
  }
}
