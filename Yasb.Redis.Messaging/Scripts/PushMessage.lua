redis.call('LPUSH',KEYS[1],KEYS[2])
redis.call("HMSET",KEYS[2],"message",ARGV[1],"contentType",ARGV[2],"from",ARGV[3],"to",ARGV[4],"retriesNumber",ARGV[5])