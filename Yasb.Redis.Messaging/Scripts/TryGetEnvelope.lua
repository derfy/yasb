local queued = redis.call('RPOPLPUSH',KEYS[1],KEYS[1])
if queued then
	local envelope = cjson.decode(queued)
	local results = redis.call("HMGET",envelope.Id,"completeTimestamp","startTimestamp","retriesNumber")
	local completeTimestamp = results[1]
	if not completeTimestamp then
		local startTimestamp = results[2]
		local retriesNumber = 0
		if results[3] then
			retriesNumber=tonumber(results[3])
		end
		if startTimestamp then 
			if tonumber(ARGV[1]) >  tonumber(startTimestamp) then
				redis.call("HSET",envelope.Id,"lastErrorMessage","Operation Timed Out")
				redis.log(redis.LOG_WARNING,"Id : "..envelope.Id.." old startime : "..startTimestamp.." now is :"..ARGV[2].." time out")
			else
				return nil
			end
		end
		if retriesNumber < 5 then
			redis.log(redis.LOG_WARNING,"Id : "..envelope.Id.." started procesing "..ARGV[2].." retries : "..retriesNumber+1)
			redis.call("HMSET",envelope.Id,"startTimestamp",ARGV[2],"retriesNumber",retriesNumber+1)
			return queued
		end
	end
	redis.call('DEL',envelope.Id)
	redis.call('LPOP',KEYS[1])
	return nil
end