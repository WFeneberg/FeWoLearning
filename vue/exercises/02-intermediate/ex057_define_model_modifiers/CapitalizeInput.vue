<!-- Exercise 057 — CapitalizeInput component (intermediate). -->
<!-- Goal:   support a custom `capitalize` modifier on defineModel, so a
             parent using `v-model.capitalize="..."` gets the first letter
             of the bound value uppercased whenever the input changes. -->
<!-- Drills: defineModel with modifiers, the [name, "modifiers"] set/get form,
             transforming values on write. -->
<script setup lang="ts">
defineProps<{ label: string }>();

// TODO: replace this with
//   const [model, modifiers] = defineModel<string, "capitalize">({ default: "" });
// and, when `modifiers.capitalize` is set, transform the value written into
// the model so its first character is uppercased. Bind the <input> below to
// `model` with v-model.
throw new Error("TODO: implement defineModel() with the capitalize modifier");
</script>

<template>
  <label>
    <span data-testid="label">{{ label }}</span>
    <input type="text" data-testid="input" />
  </label>
</template>
