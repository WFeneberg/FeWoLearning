<!-- Exercise 017 — EmitsTypedEvent component (beginner). -->
<!-- Goal:   declare a typed `submit` emit and fire it with a payload on click. -->
<!-- Drills: defineEmits with TypeScript generic syntax, emitting a payload. -->
<script setup lang="ts">
import { ref } from "vue";

interface Emits {
  submit: [text: string];
}

const emit = defineEmits<Emits>();

const text = ref("hello");

function onClick(): void {
  throw new Error("TODO: implement onClick to emit 'submit' with the text payload");
  emit("submit", text.value);
}
</script>

<template>
  <button @click="onClick">Submit</button>
</template>
