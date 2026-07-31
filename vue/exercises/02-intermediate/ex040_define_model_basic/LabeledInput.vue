<!-- Exercise 040 — LabeledInput component (intermediate). -->
<!-- Goal:   two-way bind a text input using defineModel() instead of
             manual props/emits ("modelValue" + "update:modelValue"). -->
<!-- Drills: defineModel, v-model on a custom component, script setup props. -->
<script setup lang="ts">
defineProps<{ label: string }>();

// TODO: replace this with `const model = defineModel<string>({ default: "" })`
// and bind the <input> below to it with v-model, so parent <-> child
// two-way binding works without manually declaring modelValue/emits.
throw new Error("TODO: implement defineModel() two-way binding");
</script>

<template>
  <label>
    <span data-testid="label">{{ label }}</span>
    <input type="text" data-testid="input" />
  </label>
</template>
