local results = redis.call("HMGET","hqueue",KEYS[1]..":completedTime",KEYS[1]..":startTime",KEYS[1]..":retriesNumber")
if not results[1] and not results[2] and (not results[3] or tonumber(results[3]) < 5) then
	redis.call("HINCRBY","hqueue",KEYS[1]..":retriesNumber",1)
	redis.call("HSET","hqueue",KEYS[1]..":startTime",ARGV[1])
    return true	
end


					

