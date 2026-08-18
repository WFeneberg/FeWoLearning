// Exercise 012 - named constructors & initializer lists (beginner).
//
// Goal:   Model a temperature that can be built from Celsius directly, or
//         converted from Fahrenheit in its initializer list.
// Drills: named constructors, constructor initializer lists.
// Passes: when Temperature.fahrenheit() converts to Celsius before the
//         final field is set, without a body reassignment.

double _fahrenheitToCelsius(double fahrenheit) {
  throw UnimplementedError('TODO');
}

class Temperature {
  final double celsius;

  Temperature.celsius(this.celsius);

  Temperature.fahrenheit(double fahrenheit)
      : celsius = _fahrenheitToCelsius(fahrenheit);
}
