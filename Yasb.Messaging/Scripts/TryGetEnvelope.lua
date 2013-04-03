local length=redis.call("LLEN",KEYS[1])
while length > 0 do
	local queued = redis.call('RPOP',KEYS[1])
	local envelope = cjson.decode(queued)
	local results = redis.call("HMGET","hqueue",envelope.Id..":completedTime",envelope.Id..":lastErrorMessage")
	local completedTime=results[1] local lastErrorMessage=results[2]
	if not completedTime then
		envelope.RetriesNumber=envelope.RetriesNumber+1
		envelope.LastErrorMessage=nil
		if lastErrorMessage then
			envelope.LastErrorMessage=lastErrorMessage
		elseif envelope.StartTimestamp and tonumber(ARGV[1]) >  tonumber(envelope.StartTimestamp) then
			envelope.LastErrorMessage="Operation timed out"
		end
		if not envelope.StartTimestamp or envelope.LastErrorMessage then
			envelope.StartTimestamp=ARGV[2]
			local modifiedQueued = cjson.encode(envelope)
			redis.call('LPUSH',KEYS[1],modifiedQueued)
			return modifiedQueued
		end
		redis.call('LPUSH',KEYS[1],queued)
	end
	length=length -1
end