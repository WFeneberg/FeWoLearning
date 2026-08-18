// Exercise 048 - destructuring patterns & records (reference solution).

List<String> formatScores(List<(String, int)> entries) => [
      for (final (name, score) in entries) '$name: $score',
    ];
