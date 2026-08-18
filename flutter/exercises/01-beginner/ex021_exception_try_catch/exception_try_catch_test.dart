import 'package:test/test.dart';

import 'exception_try_catch.dart';

void main() {
  test('describeDivision returns the quotient for valid input', () {
    expect(describeDivision(10, 2), 'Result: 5');
  });

  test('describeDivision reports division by zero', () {
    expect(describeDivision(10, 0), 'Error: division by zero');
  });

  test('Account.charge deducts a valid amount', () {
    final account = Account(100);
    account.charge(30);
    expect(account.balance, 70);
  });

  test('Account.charge throws NegativeAmountException for negative amounts',
      () {
    final account = Account(100);
    expect(() => account.charge(-5),
        throwsA(isA<NegativeAmountException>()));
  });

  test('Account.charge runs finally even when it throws', () {
    final account = Account(100);
    expect(() => account.charge(-5),
        throwsA(isA<NegativeAmountException>()));
    expect(account.finallyRuns, 1);
    expect(account.balance, 100);
  });
}
