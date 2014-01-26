redis.call("HSET",KEYS[1],"completeTimestamp",ARGV[1])

redis.log(redis.LOG_WARNING,"completing Id : "..KEYS[1])