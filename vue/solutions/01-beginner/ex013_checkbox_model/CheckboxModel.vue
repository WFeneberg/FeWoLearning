<!-- Exercise 013 — CheckboxModel component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const agreed = ref(false);
</script>

<template>
  <div>
    <input v-model="agreed" type="checkbox" data-testid="agreed-checkbox" />
    <p data-testid="status">{{ agreed ? "Agreed" : "Not agreed" }}</p>
  </div>
</template>
