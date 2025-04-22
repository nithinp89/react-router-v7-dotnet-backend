// app/services/cache/cache.server.ts
import Redis from "ioredis";
import type { Redis as RedisClient, RedisOptions } from "ioredis";

const defaultExpirySeconds = 3600; // 1 hour

export class CacheService {
  private redis: RedisClient;

  constructor(options?: RedisOptions) {
    this.redis = new Redis({
      host: process.env.REDIS_HOST ?? "127.0.0.1",
      port: Number(process.env.REDIS_PORT) ?? 6379,
      // password: process.env.REDIS_PASSWORD, // Uncomment if you use a password
      // db: Number(process.env.REDIS_DB) ?? 0, // Optional: select Redis DB
      ...options,
    });
  }

  /**
   * Store data in Redis by key. Data is stringified as JSON.
   * @param key Redis key
   * @param data Serializable data to store
   * @param expirySeconds Optional expiry in seconds (default: 1 hour)
   */
  async set<T>(key: string, data: T, expirySeconds?: number): Promise<void> {
    const value = JSON.stringify(data);
    expirySeconds ??= defaultExpirySeconds;
    await this.redis.set(key, value, "EX", expirySeconds);
  }

  /**
   * Retrieve data from Redis by key. Parses JSON if found, returns null if not found.
   * @param key Redis key
   */
  async get<T>(key: string): Promise<T | null> {
    const value = await this.redis.get(key);
    if (!value) return null;
    return JSON.parse(value) as T;
  }
}

// Export a singleton instance for app-wide use
export const cacheService = new CacheService();