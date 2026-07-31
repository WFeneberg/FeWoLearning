<!-- Exercise 013 — CheckboxModel component (beginner). -->
<!-- Goal:   bind a checkbox with v-model to a boolean `agreed` ref. -->
<!-- Drills: ref, v-model on <input type="checkbox">, template interpolation. -->
<script setup lang="ts">
import { ref } from "vue";

const agreed = ref(false);

// TODO: bind the checkbox below to `agreed` with v-model so the status
// text updates when the checkbox is toggled.
throw new Error("TODO: implement v-model binding on the checkbox");
</script>

<template>
  <div>
    <input type="checkbox" data-testid="agreed-checkbox" />
    <p data-testid="status">{{ agreed ? "Agreed" : "Not agreed" }}</p>
  </div>
</template>
