import { Component, signal } from "@angular/core";

// Exercise 061 — querying projected content (intermediate).
// Goal:   reach the components a caller projected in, so a container can coordinate them.
// Drills: contentChild(), contentChildren(), the content-vs-view distinction, and driving projected
//         children from their container.
// Passes: when `npx jest exercises/02-intermediate/ex061_content_child_query` is green.
//
// viewChild (exercise 024) finds what a component declared in its *own* template. contentChild
// finds what someone else projected *into* it. Two different queries because they search two
// different trees, and using the wrong one silently finds nothing — which is the single most
// common confusion here.
//
// This is what makes a container component possible. TabSet does not know what tabs exist; it
// queries for whatever was projected, then drives them. That is how a tab group, an accordion or a
// stepper is built without the caller having to wire anything up.
//
// The relationship is deliberately one-way: the container reads and drives its children, and the
// children know nothing about it. Injecting the parent into the child would work and would also
// mean the child could never be used anywhere else.
//
// Template contracts the spec asserts (classes are the query hooks — keep them):
//
// TabComponent:
//   @if (active()) {
//     <div class="tab-body"><ng-content /></div>
//   }
//
// TabSetComponent:
//   <div class="tabs">
//     @for (tab of tabList(); track tab) {
//       <button class="tab-button" type="button" (click)="select(tab.label())">
//         {{ tab.label() }}
//       </button>
//     }
//   </div>
//   <div class="panels"><ng-content /></div>
//   <p class="active">{{ activeLabel() }}</p>

@Component({
  selector: "app-tab",
  standalone: true,
  template: `<p>TODO: render the tab — see the template contract above</p>`,
})
export class TabComponent {
  /** TODO: a required input for the label. Declared as a signal so the stub compiles. */
  readonly label = signal("");

  /** Whether this tab's body is shown. Driven by the containing TabSet, not by the tab. */
  readonly active = signal(false);
}

@Component({
  selector: "app-tab-set",
  standalone: true,
  template: `<p>TODO: render the tab set — see the template contract above</p>`,
})
export class TabSetComponent {
  /**
   * TODO: query every projected TabComponent with contentChildren().
   *
   * Declared as a plain signal so the stub compiles — replace the declaration.
   */
  readonly tabs = signal<readonly TabComponent[]>([]);

  /**
   * TODO: query the first projected TabComponent with contentChild().
   *
   * Used to decide which tab starts active.
   */
  readonly firstTab = signal<TabComponent | undefined>(undefined);

  /** The projected tabs, in order. */
  tabList(): readonly TabComponent[] {
    throw new Error("TODO: implement tabList");
  }

  /** The label of the active tab, or "" when none is. */
  activeLabel(): string {
    throw new Error("TODO: implement activeLabel");
  }

  /** Activate the tab with this label and deactivate the rest. An unknown label is a no-op. */
  select(label: string): void {
    throw new Error("TODO: implement select");
  }

  /** Activate the first projected tab. Does nothing when there are none. */
  selectFirst(): void {
    throw new Error("TODO: implement selectFirst");
  }
}
