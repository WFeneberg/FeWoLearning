// Exercise 028 - string manipulation (reference solution).

List<String> splitAndTrim(String csvLine) =>
    csvLine.split(',').map((s) => s.trim()).toList();

String padId(int id) => id.toString().padLeft(4, '0');
