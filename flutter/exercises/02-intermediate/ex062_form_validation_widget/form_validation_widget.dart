// Exercise 062 - Form/TextFormField validation (intermediate).
//
// Goal:   Write a validator that rejects strings without "@", and a form
//         that shows "Valid"/"Invalid email" after Submit is tapped.
// Drills: Form, TextFormField, GlobalKey<FormState>, FormFieldValidator.
// Passes: when emailValidator rejects a value without "@" and accepts one
//         with "@", and EmailForm shows the matching message after Submit
//         runs formKey.currentState!.validate().

import 'package:flutter/material.dart';

String? emailValidator(String? value) {
  throw UnimplementedError('TODO');
}

class EmailForm extends StatefulWidget {
  const EmailForm({super.key});

  @override
  State<EmailForm> createState() => _EmailFormState();
}

class _EmailFormState extends State<EmailForm> {
  final _formKey = GlobalKey<FormState>();
  String _message = '';

  @override
  Widget build(BuildContext context) {
    throw UnimplementedError('TODO');
  }
}
