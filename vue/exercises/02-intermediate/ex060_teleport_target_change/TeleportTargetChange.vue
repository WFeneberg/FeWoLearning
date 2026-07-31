<!--
  Exercise 060 — Teleport with dynamic target/disabled (intermediate).
  Goal:   render an overlay panel that either stays in place or is teleported
          to an external target element, driven by a `useOverlay` ref.
  Drills: <Teleport :disabled>, computed booleans, DOM-location assertions.
-->
<script lang="ts">
// The CSS selector of the element the panel teleports into when open.
export const TELEPORT_TARGET = "#teleport-target-060";
</script>

<script setup lang="ts">
import { ref, computed } from "vue";

function useOverlay(initial = false) {
  const isOpen = ref(initial);
  const toggle = () => {
    isOpen.value = !isOpen.value;
  };
  return { isOpen, toggle };
}

const { isOpen, toggle } = useOverlay();

// TODO: `disabled` should be true (render in place) while the overlay is
// closed, and false (teleport to TELEPORT_TARGET) while it is open.
const disabled = computed<boolean>(() => {
  throw new Error("TODO: implement disabled computed based on isOpen");
});
</script>

<template>
  <div class="host">
    <button type="button" data-testid="toggle" @click="toggle">
      {{ isOpen ? "Close" : "Open" }}
    </button>
    <Teleport :to="TELEPORT_TARGET" :disabled="disabled">
      <div data-testid="content" class="panel">Overlay content</div>
    </Teleport>
  </div>
</template>
