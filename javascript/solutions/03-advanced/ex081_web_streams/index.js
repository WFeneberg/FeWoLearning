// Reference solution — exercise 081.

export function streamFrom(values) {
  let index = 0;
  return new ReadableStream({
    pull(controller) {
      // One chunk per pull: the consumer's demand drives the source.
      if (index >= values.length) controller.close();
      else controller.enqueue(values[index++]);
    },
  });
}

export async function collectStream(stream) {
  const chunks = [];
  for await (const chunk of stream) chunks.push(chunk);
  return chunks;
}

export function mapStream(fn) {
  return new TransformStream({
    transform(chunk, controller) {
      controller.enqueue(fn(chunk));
    },
  });
}

export async function takeAndCancel(stream, count) {
  const reader = stream.getReader();
  const chunks = [];
  while (chunks.length < count) {
    const { value, done } = await reader.read();
    if (done) break;
    chunks.push(value);
  }
  await reader.cancel();
  return { chunks, cancelled: true };
}
