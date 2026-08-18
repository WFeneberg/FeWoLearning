// Exercise 008 - named vs positional parameters (beginner).
//
// Goal:   Format a postal address from required positional parameters plus
//         a required named parameter and an optional named parameter with
//         a default.
// Drills: named vs positional parameters, `required`, default values on
//         named parameters.
// Passes: when formatAddress() places street/city positionally and
//         postalCode/country by name, defaulting country to "Germany".

String formatAddress(
  String street,
  String city, {
  required String postalCode,
  String country = 'Germany',
}) {
  throw UnimplementedError('TODO');
}
