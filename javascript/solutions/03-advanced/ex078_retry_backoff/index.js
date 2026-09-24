// Reference solution — exercise 078.

export async function retry(fn, options) {
  const {
    attempts = 3,
    baseDelay = 10,
    factor = 2,
    sleep,
    shouldRetry = () => true,
  } = options;

  let lastError;
  for (let attempt = 1; attempt <= attempts; attempt++) {
    try {
      return await fn(attempt);
    } catch (error) {
      lastError = error;
      if (attempt === attempts || !shouldRetry(error, attempt)) break;
      await sleep(baseDelay * factor ** (attempt - 1));
    }
  }
  throw lastError;
}
