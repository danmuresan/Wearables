class JsonSerializationTests {

	static function testAccel()
	{
		var size = 100;
		var accelBuffer = new AccelerationBuffer(size);
		
		for (var i = 0; i < size; i++)
		{
			var x = new Acceleration(10.433, 130.213, 1.432);
			accelBuffer.addSample(x);
		}

		HttpMbRequestsHelper.bufferToJson(accelBuffer);
	}

}