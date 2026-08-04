import { Injectable } from "@angular/core";
import { Observable, Subject } from "rxjs";

/**
 * A loader the spec drives by hand, so the in-flight window is observable.
 *
 * Already written — the exercise is the component.
 */
@Injectable({ providedIn: "root" })
export class ArticleLoader {
  readonly requested: string[] = [];

  readonly cancelled: string[] = [];

  private readonly pending = new Map<string, Subject<string>>();

  load(id: string): Observable<string> {
    this.requested.push(id);
    const subject = new Subject<string>();
    let finished = false;
    this.pending.set(id, subject);
    return new Observable<string>((subscriber) => {
      const subscription = subject.subscribe({
        next: (value) => subscriber.next(value),
        error: (error: unknown) => subscriber.error(error),
        complete: () => {
          finished = true;
          subscriber.complete();
        },
      });
      return () => {
        if (!finished) {
          this.cancelled.push(id);
        }
        subscription.unsubscribe();
      };
    });
  }

  /** Answer an outstanding load with a title. */
  respond(id: string, title: string): void {
    const subject = this.pending.get(id);
    if (subject === undefined) {
      throw new Error(`no outstanding load for "${id}"`);
    }
    subject.next(title);
    subject.complete();
    this.pending.delete(id);
  }

  isPending(id: string): boolean {
    return this.pending.has(id);
  }
}
