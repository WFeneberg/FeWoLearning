// Exercise 035 - package:test basics (beginner).
//
// Goal:   Implement Counter.reset() so it zeroes the counter. Read the
//         sibling test file too — it's part of this exercise, demonstrating
//         group()/setUp()/tearDown() from package:test.
// Drills: package:test group/setUp/tearDown (see the test file), basic
//         state mutation.
// Passes: when reset() sets value back to 0 regardless of its current value.

class Counter {
  int value = 0;

  void increment() {
    value++;
  }

  void reset() {
    throw UnimplementedError('TODO');
  }
}
