// Exercise 025 - fold vs reduce (reference solution).

int sumWithFold(List<int> numbers) =>
    numbers.fold(0, (acc, n) => acc + n);

int maxWithReduce(List<int> numbers) =>
    numbers.reduce((a, b) => a > b ? a : b);

String joinWithFold(List<String> words) =>
    words.fold('', (acc, w) => acc.isEmpty ? w : '$acc, $w');
