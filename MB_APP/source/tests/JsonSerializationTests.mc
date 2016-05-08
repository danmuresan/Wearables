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

		Toybox.System.println(HttpMbRequestsHelper.bufferToJson(accelBuffer));
	}

	static function testHr()
	{
		var size = 5;
		var buffer = new HrBuffer(size);
		
		for (var i = 0; i < size; i++)
		{
			var x = buffer.getElementAt(i);
			buffer.addSample(x);
		}

		Toybox.System.println(HttpMbRequestsHelper.bufferToJson(buffer));
	}
}