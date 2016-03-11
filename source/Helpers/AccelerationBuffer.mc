class AccelerationBuffer extends SensorDataBuffer {
	
	const ACCEL_BUFFER_SIZE = 1000;
	
	hidden var accelBufferIndex;
	hidden var accelBuffer;
	
	function initialize() {
		accelBuffer = new [ACCEL_BUFFER_SIZE];
		accelBufferIndex = 0;
	}
	
	function addSample( acc ) {
		if (accelBufferIndex < ACCEL_BUFFER_SIZE)
		{
			accelBuffer[accelBufferIndex] = acc;
			accelBufferIndex++;
		}
		else
		{
			System.println( "Reached acc buffer limit, reseting index." );
			accellBufferIndex = 0;
			accelBuffer[accelBufferIndex] = acc;
			accelBufferIndex++;
		}
	}
	 
 	function getBufferLength() {
		return accelBufferIndex;
	}
	
	function hasReachedBufferLimit() {
		return accelBufferIndex == ACCEL_BUFFER_SIZE - 1; 
	}
	
}