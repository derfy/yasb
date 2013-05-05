﻿local queued = redis.call('RPOP',KEYS[1])
if queued then
	local envelope = cjson.decode(queued)
	local results = redis.call("HMGET","hqueue",envelope.Id..":completedTime",envelope.Id..":lastErrorMessage")
	local completedTime=results[1] local lastErrorMessage=results[2]
	if not completedTime then
		envelope.LastErrorMessage=lastErrorMessage
		if envelope.StartTimestamp then 
			if tonumber(ARGV[1]) >  tonumber(envelope.StartTimestamp) then
				envelope.LastErrorMessage="Operation Timed Out"
			else
				redis.call('LPUSH',KEYS[1],queued)
				return nil
			end
		end
		if envelope.RetriesNumber < 5 then
			envelope.RetriesNumber=envelope.RetriesNumber+1
			envelope.StartTimestamp=ARGV[2]
			local modifiedQueued = cjson.encode(envelope)
			redis.call('LPUSH',KEYS[1],modifiedQueued)
			return modifiedQueued
		end
	end
end