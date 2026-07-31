import { createApp, defineAsyncComponent, h } from "vue";
import { JSDOM } from "jsdom";

const dom = new JSDOM("<!doctype html><div id='app'></div>");
global.window = dom.window;
global.document = dom.window.document;
global.SVGElement = dom.window.SVGElement;
global.Node = dom.window.Node;
global.Element = dom.window.Element;

let attempts = 0;
const loader = () => {
  attempts++;
  console.log("loader call", attempts);
  if (attempts === 1) return Promise.reject(new Error("fail1"));
  return Promise.resolve({ render: () => h("div", "OK") });
};

const Comp = defineAsyncComponent({
  loader,
  loadingComponent: { render: () => h("div", "Loading") },
  errorComponent: { render: () => h("div", "Error") },
  delay: 0,
  onError(err, retry, fail, n) {
    console.log("onError", n);
    if (n <= 1) retry(); else fail();
  },
});

const app = createApp({ render: () => h(Comp) });
app.mount("#app");

setTimeout(() => {
  console.log("RESULT", document.getElementById("app").innerHTML);
}, 200);
