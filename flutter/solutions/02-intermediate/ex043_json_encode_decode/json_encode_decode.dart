// Exercise 043 - dart:convert basics (reference solution).

import 'dart:convert';

String encodeUser(Map<String, Object?> user) => jsonEncode(user);

Map<String, Object?> decodeUser(String json) =>
    jsonDecode(json) as Map<String, Object?>;
