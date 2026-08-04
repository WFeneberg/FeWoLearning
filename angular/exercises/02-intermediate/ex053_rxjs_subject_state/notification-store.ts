import { Injectable } from "@angular/core";
import { Observable } from "rxjs";

// Exercise 053 — Subjects as state (intermediate).
// Goal:   push state imperatively and let consumers observe it, without leaking the ability to write.
// Drills: BehaviorSubject vs Subject vs ReplaySubject, asObservable() to seal the write end,
//         reading .value, and why a late subscriber sees different things for each.
// Passes: when `npx jest exercises/02-intermediate/ex053_rxjs_subject_state` is green.
//
// A Subject is both an observer and an observable, which is exactly what makes it dangerous to
// expose. Hand one out and any consumer can call next() on it, so the store no longer owns its
// own state. `asObservable()` is the seal: same stream, no write end.
//
// The three variants differ only in what a *late* subscriber gets, and that difference is the
// whole reason to choose between them:
//
//   Subject          — nothing. Values emitted before you subscribed are gone. Right for events
//                      ("a save happened"), wrong for state, because a component created after
//                      the value was set renders empty.
//   BehaviorSubject  — the current value, immediately. Right for state, and it needs an initial
//                      value precisely because "current" has to mean something at time zero.
//   ReplaySubject(n) — the last n values. Right for a short history, such as a toast log.
//
// The failure this prevents is subtle: with a plain Subject everything works while the component
// happens to be created before the first emission, and breaks the moment it is not — a bug that
// shows up as "empty on refresh" and nowhere else.

export interface Notice {
  readonly id: number;
  readonly text: string;
}

@Injectable({ providedIn: "root" })
export class NotificationStore {
  /**
   * TODO: three private subjects — a BehaviorSubject<number> for the unread count starting at 0,
   * a Subject<Notice> for arrivals, and a ReplaySubject<Notice> keeping the last 3.
   *
   * All private: the public surface is the observables below.
   */

  /** TODO: the unread count as a read-only stream. */
  readonly unreadCount$: Observable<number> = new Observable<number>();

  /** TODO: arrivals as a read-only stream. */
  readonly arrivals$: Observable<Notice> = new Observable<Notice>();

  /** TODO: the last three notices as a read-only stream. */
  readonly recent$: Observable<Notice> = new Observable<Notice>();

  /** The current unread count, read synchronously — what BehaviorSubject.value is for. */
  currentCount(): number {
    throw new Error("TODO: implement currentCount");
  }

  /** Push a notice: emit it on both arrival streams and bump the unread count. */
  notify(notice: Notice): void {
    throw new Error("TODO: implement notify");
  }

  /** Set the unread count back to zero. */
  markAllRead(): void {
    throw new Error("TODO: implement markAllRead");
  }

  /**
   * Whether the public streams are write-sealed.
   *
   * TODO: return true when none of the three public streams has a `next` method — that is, when
   * each one went through asObservable() rather than being handed out raw.
   */
  isSealed(): boolean {
    throw new Error("TODO: implement isSealed");
  }
}
