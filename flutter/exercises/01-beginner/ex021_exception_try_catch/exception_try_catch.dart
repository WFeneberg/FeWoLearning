// Exercise 021 - exception handling with try/catch/finally (beginner).
//
// Goal:   Report the result of an integer division, catching the built-in
//         division-by-zero error, and charge an account balance, rejecting
//         negative amounts with a custom exception while still running
//         cleanup code via finally.
// Drills: try/catch/finally, custom exceptions.
// Passes: when describeDivision() handles IntegerDivisionByZeroException and
//         Account.charge() throws NegativeAmountException for negative
//         amounts while incrementing finallyRuns even on that failure path.

class NegativeAmountException implements Exception {
  final String message;
  NegativeAmountException(this.message);

  @override
  String toString() => 'NegativeAmountException: $message';
}

class Account {
  int balance;
  int finallyRuns = 0;

  Account(this.balance);

  void charge(int amount) {
    throw UnimplementedError('TODO');
  }
}

String describeDivision(int a, int b) {
  throw UnimplementedError('TODO');
}
