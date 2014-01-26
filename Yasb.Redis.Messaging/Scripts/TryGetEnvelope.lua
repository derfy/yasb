local queued = redis.call('RPOP',KEYS[1])
if queued then
	local envelope = cjson.decode(queued)
	local results = redis.call("HMGET",envelope.Id,"completeTimestamp","lastErrorMessage")
	local completeTimestamp = results[1]
	envelope.LastErrorMessage = results[2]
	if not completeTimestamp then
		local startTimestamp = envelope.StartTimestamp
		local retriesNumber = tonumber(envelope.RetriesNumber)
		if startTimestamp then 
			if tonumber(ARGV[1]) >  tonumber(startTimestamp) then
				envelope.LastErrorMessage="Operation Timed Out"
				redis.log(redis.LOG_WARNING,"Id : "..envelope.Id.." timed out : starTimestamp : "..startTimestamp.." currentTimestamp :"..ARGV[2])
			else
				redis.call('LPUSH',KEYS[1],queued)
				return nil
			end
		end
		if retriesNumber < 5 then
		    redis.log(redis.LOG_WARNING,"starting Id : "..envelope.Id.." at : starTimestamp : "..ARGV[2])
			envelope.StartTimestamp=ARGV[2]
			envelope.RetriesNumber=retriesNumber+1
			queued = cjson.encode(envelope)
			redis.call('LPUSH',KEYS[1],queued)
			return queued
		end
	end
	redis.call('DEL',envelope.Id)
	return nil
end