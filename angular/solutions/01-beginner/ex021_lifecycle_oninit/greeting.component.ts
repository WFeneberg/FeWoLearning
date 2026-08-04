import { Component, Input, OnInit } from "@angular/core";

// Exercise 021 — ngOnInit (reference solution).
@Component({
  selector: "app-greeting",
  standalone: true,
  template: `
    <p class="greeting">{{ greeting }}</p>
    <p class="order">{{ log.join(" > ") }}</p>
  `,
})
export class GreetingComponent implements OnInit {
  @Input() name?: string;
  @Input() salutation = "Hello";

  readonly log: string[] = [];

  nameAtConstruction?: string;
  nameAtInit?: string;

  greeting = "";

  constructor() {
    this.log.push("constructor");
    // Angular has not written the inputs yet, so this is undefined every time. Capturing
    // it is the whole demonstration — real setup code must not read inputs here.
    this.nameAtConstruction = this.name;
  }

  ngOnInit(): void {
    this.log.push("ngOnInit");
    // By now the inputs have been set, so this is the place for one-time setup.
    this.nameAtInit = this.name;
    this.greeting = `${this.salutation}, ${this.name ?? "guest"}!`;
  }
}
