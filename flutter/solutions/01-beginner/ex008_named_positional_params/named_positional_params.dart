// Exercise 008 - named vs positional parameters (reference solution).

String formatAddress(
  String street,
  String city, {
  required String postalCode,
  String country = 'Germany',
}) =>
    '$street, $postalCode $city, $country';
