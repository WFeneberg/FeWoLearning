// Exercise 001 - var/final/const basics (reference solution).

const double taxRate = 0.19;

int area(int width, int height) => width * height;

int perimeter(int width, int height) => 2 * (width + height);

double priceWithTax(double basePrice) => basePrice * (1 + taxRate);
