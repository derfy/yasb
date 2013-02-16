local length=redis.call("LLEN",KEYS[1])
while length > 0 do
	local queued = redis.call('RPOPLPUSH',KEYS[1],KEYS[1])
    local envelope = cjson.decode(queued)
    local results = redis.call("HMGET","hqueue",envelope.Id..":completedTime",envelope.Id..":startTime",envelope.Id..":retriesNumber")
	local completedTime = results[1] local startTime = results[2] local retriesNumber=results[3]
	if not completedTime then
		if not startTime then
			if not retriesNumber or tonumber(retriesNumber) < 5 then
				return queued
			else
				redis.call('LPOP',KEYS[1])
				redis.call('LPUSH',"deadLetter",queued)
			end
		elseif tonumber(ARGV[1]) >  tonumber(startTime) then
			redis.call("HDEL","hqueue",envelope.Id..":startTime")
		end
	else
		redis.call('LPOP',KEYS[1])
		redis.call("HDEL","hqueue",envelope.Id..":startTime",envelope.Id..":retriesNumber")
	end
    length=length -1
end