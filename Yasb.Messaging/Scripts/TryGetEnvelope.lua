local length=redis.call("LLEN",KEYS[1])
while length > 0 do
	local queued = redis.call('RPOP',KEYS[1])
	local envelope = cjson.decode(queued)
	local completedTime = redis.call("HGET","hqueue",envelope.Id..":completedTime")
	if not completedTime then
		if tonumber(envelope.RetriesNumber) < 5 then
			local modifiedQueued=queued
			envelope.RetriesNumber=envelope.RetriesNumber+1
			if not envelope.StartTime then
				envelope.StartTime=ARGV[1]
				modifiedQueued = cjson.encode(envelope)
				redis.call('LPUSH',KEYS[1],modifiedQueued)
				return modifiedQueued
			elseif tonumber(ARGV[1]) >  tonumber(envelope.StartTime) then
				envelope.StartTime=nil
				modifiedQueued=cjson.encode(envelope)
			end
			redis.call('LPUSH',KEYS[1],modifiedQueued)
		else
			redis.call('LPUSH',"deadLetter",queued)
		end
	end
	length=length -1
end