<!-- Exercise 009 — ClassBinding component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const isActive = ref(false);

function toggle() {
  isActive.value = !isActive.value;
}
</script>

<template>
  <div>
    <div class="box" :class="{ active: isActive }" data-testid="box">Box</div>
    <button @click="toggle">Toggle</button>
  </div>
</template>
