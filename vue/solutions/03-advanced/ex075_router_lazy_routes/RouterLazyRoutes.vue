<!-- Exercise 075 — RouterLazyRoutes component (reference solution). -->
<script setup lang="ts">
import { computed, defineAsyncComponent, h, ref, type Component } from "vue";

// A minimal stand-in for Vue Router's `RouteRecordRaw`: each route's
// `component` is a lazy loader function, exactly like the dynamic
// `() => import("./views/Settings.vue")` you would write with real Vue
// Router to code-split a route.
export interface LazyRouteRecord {
  path: string;
  component: () => Promise<Component | { default: Component }>;
}

// The lazily-loaded "chunks". In a real app these would be
// `() => import("./views/Home.vue")` / `() => import("./views/Settings.vue")`;
// here they are inlined as on-demand components so the exercise needs no
// extra files, but each loader still returns a Promise, so it behaves
// exactly like a dynamic import resolving once its chunk arrives.
const routes: LazyRouteRecord[] = [
  {
    path: "/",
    component: () =>
      Promise.resolve({
        name: "Home",
        setup() {
          return () => h("p", { class: "home-view" }, "Home page");
        },
      }),
  },
  {
    path: "/settings",
    component: () =>
      Promise.resolve({
        name: "Settings",
        setup() {
          return () => h("p", { class: "settings-view" }, "Settings page loaded lazily");
        },
      }),
  },
];

const currentPath = ref("/");

const matchedComponent = computed((): ReturnType<typeof defineAsyncComponent> => {
  const route = routes.find((r) => r.path === currentPath.value) ?? routes[0]!;
  return defineAsyncComponent(route.component);
});

function navigate(path: string): void {
  currentPath.value = path;
}

defineExpose({ currentPath, navigate });
</script>

<template>
  <component :is="matchedComponent" />
</template>
