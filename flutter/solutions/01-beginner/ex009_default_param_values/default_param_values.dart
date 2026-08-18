// Exercise 009 - default parameter values (reference solution).

String repeatJoin(String value, [int times = 1, String separator = ', ']) =>
    List.filled(times, value).join(separator);
