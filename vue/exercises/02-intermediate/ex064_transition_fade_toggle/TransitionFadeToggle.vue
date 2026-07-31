<!-- Exercise 064 — TransitionFadeToggle component (intermediate). -->
<!-- Goal:   wrap a v-if toggled message in <Transition name="fade">. -->
<!-- Drills: Transition component, enter/leave class naming, v-if inside Transition. -->
<script setup lang="ts">
import { ref } from "vue";

const visible = ref(false);

function toggle(): void {
  throw new Error("TODO: implement toggle (flip visible)");
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
