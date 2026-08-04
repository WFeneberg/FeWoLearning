import { Component, computed, Pipe, PipeTransform, signal } from "@angular/core";

// Exercise 057 — an impure pipe, and when it is justified (reference solution).

const filterJoin = (items: readonly string[], term: string): string =>
  items.filter((item) => item.toLowerCase().includes(term.toLowerCase())).join(", ");

@Pipe({
  name: "liveFilter",
  standalone: true,
  // Called on every change-detection pass, whatever the inputs. It picks up an in-place
  // mutation, and it pays for that on every single render.
  pure: false,
})
export class LiveFilterPipe implements PipeTransform {
  static calls = 0;

  transform(items: readonly string[], term: string): string {
    LiveFilterPipe.calls += 1;
    return filterJoin(items, term);
  }
}

@Pipe({
  name: "staticFilter",
  standalone: true,
})
export class StaticFilterPipe implements PipeTransform {
  static calls = 0;

  transform(items: readonly string[], term: string): string {
    StaticFilterPipe.calls += 1;
    return filterJoin(items, term);
  }
}

@Component({
  selector: "app-live-filter-host",
  standalone: true,
  imports: [LiveFilterPipe, StaticFilterPipe],
  template: `
    <p class="impure">{{ items | liveFilter: term() }}</p>
    <p class="pure">{{ items | staticFilter: term() }}</p>
    <p class="computed">{{ filtered() }}</p>
  `,
})
export class LiveFilterHostComponent {
  items: string[] = ["apple", "banana", "cherry"];

  readonly term = signal("a");

  computedCalls = 0;

  // Memoised on `term`, which is the honest dependency. It does not track `items`, because a
  // plain mutable array cannot be tracked — the real fix there is to make it a signal.
  readonly filtered = computed(() => {
    this.computedCalls += 1;
    return filterJoin(this.items, this.term());
  });

  pushInPlace(item: string): void {
    this.items.push(item);
  }
}
