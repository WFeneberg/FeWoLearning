<!--
  Exercise 043 — Modal component (reference solution).
  Content is teleported to "#modal-root" via <Teleport>, keeping the
  component's own subtree empty while the rendered markup lives at the
  teleport target in the DOM.
-->
<template>
  <Teleport to="#modal-root">
    <div v-if="open" class="modal-overlay">
      <div class="modal-dialog" role="dialog" aria-modal="true">
        <header class="modal-header">
          <h2>{{ title }}</h2>
          <button
            type="button"
            class="modal-close"
            aria-label="Close"
            @click="$emit('close')"
          >
            &times;
          </button>
        </header>
        <div class="modal-body">
          <slot />
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
defineProps<{
  open: boolean;
  title: string;
}>();

defineEmits<{
  (e: "close"): void;
}>();
</script>
