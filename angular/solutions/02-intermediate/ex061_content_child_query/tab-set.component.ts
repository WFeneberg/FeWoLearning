import { Component, contentChild, contentChildren, input, signal } from "@angular/core";

// Exercise 061 — querying projected content (reference solution).

@Component({
  selector: "app-tab",
  standalone: true,
  template: `
    @if (active()) {
      <div class="tab-body"><ng-content /></div>
    }
  `,
})
export class TabComponent {
  readonly label = input.required<string>();

  // Written by the containing TabSet. The tab never looks upwards, which is what keeps it usable
  // anywhere else.
  readonly active = signal(false);
}

@Component({
  selector: "app-tab-set",
  standalone: true,
  template: `
    <div class="tabs">
      @for (tab of tabList(); track tab) {
        <button class="tab-button" type="button" (click)="select(tab.label())">
          {{ tab.label() }}
        </button>
      }
    </div>
    <div class="panels"><ng-content /></div>
    <p class="active">{{ activeLabel() }}</p>
  `,
})
export class TabSetComponent {
  // contentChildren, not viewChildren: these live in the *caller's* template. A view query would
  // search this component's own template and find nothing at all.
  readonly tabs = contentChildren(TabComponent);

  readonly firstTab = contentChild(TabComponent);

  tabList(): readonly TabComponent[] {
    return this.tabs();
  }

  activeLabel(): string {
    return this.tabList().find((tab) => tab.active())?.label() ?? "";
  }

  select(label: string): void {
    const target = this.tabList().find((tab) => tab.label() === label);
    if (target === undefined) {
      return;
    }
    // Deactivate everything first, so exactly one body is ever shown.
    for (const tab of this.tabList()) {
      tab.active.set(tab === target);
    }
  }

  selectFirst(): void {
    const first = this.firstTab();
    if (first === undefined) {
      return;
    }
    this.select(first.label());
  }
}
