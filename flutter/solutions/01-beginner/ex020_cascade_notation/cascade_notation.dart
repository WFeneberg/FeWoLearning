// Exercise 020 - cascade notation (reference solution).

String buildReport(String title, List<String> lines) {
  final buffer = StringBuffer()
    ..writeln(title)
    ..writeln('-' * title.length);
  for (final line in lines) {
    buffer.writeln(line);
  }
  return buffer.toString();
}
