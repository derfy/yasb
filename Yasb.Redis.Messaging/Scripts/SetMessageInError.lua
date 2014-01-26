redis.call("HDEL",KEYS[1],"startTimestamp")
redis.call("HSET",KEYS[1],"lastErrorMessage",ARGV[1])
redis.log(redis.LOG_WARNING,"Id : "..KEYS[1].." is in error :")