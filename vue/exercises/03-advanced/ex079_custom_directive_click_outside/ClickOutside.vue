<!-- Exercise 079 — ClickOutside component (advanced).
     Goal:   implement a `v-click-outside` custom directive that invokes a
             bound callback whenever a click occurs outside its element, by
             attaching a `click` listener on `document` when the element is
             mounted and removing it when the element is unmounted.
     Drills: custom directive lifecycle hooks (mounted/unmounted), global
             event listeners, DOM containment checks (`Node.contains`),
             cleaning up listeners to avoid leaks. -->
<script setup lang="ts">
import type { Directive } from "vue";

const emit = defineEmits<{ outside: [] }>();

const vClickOutside: Directive<HTMLElement, () => void> = {
  mounted(_el, _binding) {
    throw new Error("TODO: implement vClickOutside mounted hook");
  },
  unmounted(_el) {
    throw new Error("TODO: implement vClickOutside unmounted hook");
  },
};
</script>

<template>
  <div>
    <div data-testid="box" v-click-outside="() => emit('outside')">Inside</div>
    <button type="button" data-testid="outside">Outside</button>
  </div>
</template>
