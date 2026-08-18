// Exercise 021 - exception handling with try/catch/finally (reference solution).

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
    try {
      if (amount < 0) {
        throw NegativeAmountException('amount must not be negative: $amount');
      }
      balance -= amount;
    } finally {
      finallyRuns++;
    }
  }
}

String describeDivision(int a, int b) {
  try {
    return 'Result: ${a ~/ b}';
  } on IntegerDivisionByZeroException {
    return 'Error: division by zero';
  }
}
