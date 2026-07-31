<!-- Exercise 017 — EmitsTypedEvent component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

interface Emits {
  submit: [text: string];
}

const emit = defineEmits<Emits>();

const text = ref("hello");

function onClick(): void {
  emit("submit", text.value);
}
</script>

<template>
  <button @click="onClick">Submit</button>
</template>
