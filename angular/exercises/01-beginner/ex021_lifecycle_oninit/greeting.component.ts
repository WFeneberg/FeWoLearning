import { Component, Input, OnInit } from "@angular/core";

// Exercise 021 — ngOnInit (beginner).
// Goal:   learn what the constructor cannot see, and what ngOnInit can.
// Drills: implements OnInit, the constructor-vs-ngOnInit ordering, decorator inputs being
//         unset during construction, and doing setup work once inputs have arrived.
// Passes: when `npx jest exercises/01-beginner/ex021_lifecycle_oninit` is green.
//
// The rule this exercise exists to teach: a constructor runs when the class is
// instantiated, which is *before* Angular has written any @Input. So `this.name` is
// undefined in the constructor and set by the time ngOnInit runs. Setup that depends on
// an input therefore belongs in ngOnInit — putting it in the constructor is one of the
// most common Angular bugs, and it fails quietly with an undefined rather than loudly.
//
// (Signal inputs sidestep this: they are read lazily, so a computed can be declared in a
// field initialiser and still see the bound value. Exercise 007 covers those.)
//
// Template contract the spec asserts (classes are the query hooks — keep them):
//   <p class="greeting">{{ greeting }}</p>
//   <p class="order">{{ log.join(" > ") }}</p>

@Component({
  selector: "app-greeting",
  standalone: true,
  template: `<p>TODO: render the greeting — see the template contract above</p>`,
})
export class GreetingComponent implements OnInit {
  @Input() name?: string;
  @Input() salutation = "Hello";

  /** Which hooks ran, in order — "constructor" then "ngOnInit". */
  readonly log: string[] = [];

  /** What `name` looked like during construction. Expected to be undefined. */
  nameAtConstruction?: string;

  /** What `name` looked like by ngOnInit. Expected to be the bound value. */
  nameAtInit?: string;

  /** Built once in ngOnInit: `"<salutation>, <name>!"`, or `"<salutation>, guest!"`. */
  greeting = "";

  constructor() {
    // TODO: record "constructor" in the log, and capture `name` into nameAtConstruction.
    throw new Error("TODO: implement the constructor");
  }

  ngOnInit(): void {
    // TODO: record "ngOnInit", capture `name` into nameAtInit, and build the greeting.
    throw new Error("TODO: implement ngOnInit");
  }
}
