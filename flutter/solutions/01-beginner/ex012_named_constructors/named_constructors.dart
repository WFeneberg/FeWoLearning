// Exercise 012 - named constructors & initializer lists (reference solution).

double _fahrenheitToCelsius(double fahrenheit) => (fahrenheit - 32) * 5 / 9;

class Temperature {
  final double celsius;

  Temperature.celsius(this.celsius);

  Temperature.fahrenheit(double fahrenheit)
      : celsius = _fahrenheitToCelsius(fahrenheit);
}
