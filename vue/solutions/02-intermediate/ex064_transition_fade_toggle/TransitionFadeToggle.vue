<!-- Exercise 064 — TransitionFadeToggle component (reference solution). -->
<script setup lang="ts">
import { ref } from "vue";

const visible = ref(false);

function toggle(): void {
  visible.value = !visible.value;
}

defineExpose({ visible, toggle });
</script>

<template>
  <div>
    <button type="button" data-testid="toggle-btn" @click="toggle">Toggle</button>
    <Transition name="fade">
      <p v-if="visible" data-testid="message">Hello there</p>
    </Transition>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
