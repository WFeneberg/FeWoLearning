// Reference solution — exercise 017.

export function varSurvivesBlock() {
  if (true) {
    var message = "from block"; // hoisted to the top of the FUNCTION
  }
  return message;
}

export function tdzErrorName() {
  try {
    value;
    let value = 1;
    return value;
  } catch (error) {
    return error.name;
  }
}

export function callsBeforeDeclaration() {
  return helper();

  function helper() {
    return "hoisted";
  }
}

export function typeofUndeclared() {
  return typeof definitelyNotDeclaredAnywhere;
}
